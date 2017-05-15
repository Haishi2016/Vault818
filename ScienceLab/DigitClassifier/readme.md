# Digit Classifier

This is a C# implementation of a handwritten digit classifier neuron network in Michael Nielsen's Neural Networks and Deep Learning book.

 ![Digit Classifier Sample Output](../../imgs/digit-classifier-1.png)


Currently, the trained network has about 95% accuracy, which is to be improved in future iterations.

## Run the code

This project is written with .Net Framework 4.7 using Visual Studio 2017.

1. Download the [MNIST data set](http://yann.lecun.com/exdb/mnist/). Save the 4 archive files (train-images-idx3-ubyte.gz, train-labels-idx1-ubyte.gz, t10k-images-idx3-ubyte.gz, t10k-labels-idx1-ubyte.gz) to a folder of your choice.
2. Modify the first few lines of code in **Program.cs** to point to correct data package files.
3. Compile and run. The code trains on 60,000 images from the training set, and then allows you to classify an individual image from the 10,000-image test set by entering an image id (0-9,999). Or, you can enter '-1' to run against the entire test set. Enter '-2' to exit.

