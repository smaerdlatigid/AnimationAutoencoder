from tensorflow.keras.layers import Lambda, Input, Dense, Dropout
from tensorflow.keras.models import Model
from tensorflow.keras.datasets import mnist
from tensorflow.keras.losses import mse, binary_crossentropy
from tensorflow.keras.utils import plot_model
from tensorflow.keras import backend as K
from tensorflow import lite
import numpy as np

from mpl_toolkits.mplot3d import Axes3D 
import matplotlib.pyplot as plt
import argparse
import os

# based on
# https://keras.io/examples/variational_autoencoder/

# reparameterization trick
# instead of sampling from Q(z|X), sample epsilon = N(0,I)
# z = z_mean + sqrt(var) * epsilon
def sampling(args):
    """Reparameterization trick by sampling from an isotropic unit Gaussian.

    # Arguments
        args (tensor): mean and log of variance of Q(z|X)

    # Returns
        z (tensor): sampled latent vector
    """

    z_mean, z_log_var = args
    batch = K.shape(z_mean)[0]
    dim = K.int_shape(z_mean)[1]
    # by default, random_normal has mean = 0 and std = 1.0
    epsilon = K.random_normal(shape=(batch, dim))
    return z_mean + K.exp(0.5 * z_log_var) * epsilon


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    help_ = "Load training data"
    parser.add_argument("-i","--train",help=help_)
    help_ = "Load test data"
    parser.add_argument("-i2","--test",help=help_)
    help_ = "Load h5 model trained weights"
    parser.add_argument("-w", "--weights", help=help_)
    help_ = "Number of training epochs"
    parser.add_argument("-e", "--epochs", help=help_, default=50 ,type=int)
    help_ = "Use mse loss instead of binary cross entropy (default)"
    parser.add_argument("-m", "--mse", help=help_, action='store_true')
    help_ = "Number of batches per training step"
    parser.add_argument("-b", "--batch", help=help_, default=8, type=int)
    help_ = "Dimensionality of latent space"
    parser.add_argument("-l", "--latent", help=help_, default=2, type=int)
    args = parser.parse_args()

    x_train = np.loadtxt(args.train).astype(np.float32)
    x_test = np.loadtxt(args.test).astype(np.float32)

    xm = x_train.mean(0)
    x_train -= xm
    x_test -= xm
    # xs = x_train.std(0)
    # x_train /= xs
    # x_train[np.isnan(x_train)] = 0
    np.savetxt("mean_pose.txt", xm, fmt="%.3f")
    # np.savetxt("mean_std.txt", xs)
    
    # MNIST dataset
    # (x_train, y_train), (x_test, y_test) = mnist.load_data()
    # data = (x_test, y_test)

    # image_size = x_train.shape[1]
    # original_dim = image_size * image_size
    # x_train = np.reshape(x_train, [-1, original_dim])
    # x_test = np.reshape(x_test, [-1, original_dim])
    # x_train = x_train.astype('float32') / 255
    # x_test = x_test.astype('float32') / 255

    # VAE model = encoder + decoder
    # build encoder model
    inputs = Input(shape=x_train.shape[1], name='encoder_input')
    x = Dense(256, activation='relu')(inputs)
    x = Dropout(0.25)(x)
    x = Dense(128, activation='relu')(x)
    x = Dense(128, activation='relu')(x)
    x = Dense(64, activation='relu')(x)
    z_mean = Dense(args.latent, name='z_mean', activation='linear')(x)
    z_log_var = Dense(args.latent, name='z_log_var', activation='linear')(x)

    # use reparameterization trick to push the sampling out as input
    # note that "output_shape" isn't necessary with the TensorFlow backend
    z = Lambda(sampling, output_shape=(args.latent,), name='z')([z_mean, z_log_var])

    # instantiate encoder model
    encoder = Model(inputs, [z_mean, z_log_var, z], name='encoder')
    encoder.summary()
    plot_model(encoder, to_file='vae_mlp_encoder.png', show_shapes=True)

    # build decoder model
    latent_inputs = Input(shape=(args.latent,), name='z_sampling')
    x = Dense(64, activation='relu')(latent_inputs)
    x = Dense(128, activation='relu')(x)
    x = Dense(256, activation='relu')(x)
    x = Dropout(0.25)(x)
    x = Dense(512, activation='relu')(x)
    outputs = Dense(x_train.shape[1], activation='linear')(x)

    # instantiate decoder model
    decoder = Model(latent_inputs, outputs, name='decoder')
    decoder.summary()
    plot_model(decoder, to_file='vae_mlp_decoder.png', show_shapes=True)

    # instantiate VAE model
    outputs = decoder(encoder(inputs)[2])
    vae = Model(inputs, outputs, name='vae_mlp')

    models = (encoder, decoder)

    # VAE loss = mse_loss or xent_loss + kl_loss
    if args.mse:
        reconstruction_loss = mse(inputs, outputs)
    else:
        reconstruction_loss = binary_crossentropy(inputs,
                                                  outputs)

    reconstruction_loss *= x_train.shape[1]
    kl_loss = 1 + z_log_var - K.square(z_mean) - K.exp(z_log_var)
    kl_loss = K.sum(kl_loss, axis=-1)
    kl_loss *= -0.5
    vae_loss = K.mean(reconstruction_loss + kl_loss)
    vae.add_loss(vae_loss)
    vae.compile(optimizer='adam')
    vae.summary()
    plot_model(vae,
               to_file='vae_mlp.png',
               show_shapes=True)

    if args.weights:
        vae.load_weights(args.weights)
    else:
        # train the autoencoder
        vae.fit(x_train,
                epochs=args.epochs,
                batch_size=args.batch,
                validation_data=(x_test, None))

        converter = lite.TFLiteConverter.from_keras_model(decoder)
        tflite_model = converter.convert()
        open("decoder.tflite", "wb").write(tflite_model)

        try:
            vae.save_weights('vae_mlp_mnist.h5')
        except:
            encoder.save_weights('encoder.h5')
            decoder.save_weights('decoder.h5')

    z_mean, _, _ = encoder.predict(x_train[:2])
    print(z_mean)
    z_mean, _, _ = encoder.predict(x_train[-2:])
    print(z_mean)

    # TODO make encoding plot/ latent space + pose pictures, need to save from unity