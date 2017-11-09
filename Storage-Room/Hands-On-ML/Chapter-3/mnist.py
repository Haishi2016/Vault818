from sklearn.datasets import fetch_mldata
import matplotlib
import matplotlib.pyplot as plt
import numpy as np
from sklearn.linear_model import SGDClassifier
from sklearn.model_selection import cross_val_score
from sklearn.model_selection import cross_val_predict
from sklearn.metrics import confusion_matrix

mnist = fetch_mldata('MNIST original')
X, y = mnist["data"], mnist["target"]

some_digit = X[36000]
some_digit_image = some_digit.reshape(28, 28)

plt.imshow(some_digit_image, cmap = matplotlib.cm.binary, interpolation="nearest")
plt.axis("off")
plt.show()

X_train, X_test, y_train, y_test = X[:60000], X[60000:], y[:60000], y[60000:]

shuffle_index = np.random.permutation(60000)

X_train, y_train = X_train[shuffle_index], y_train[shuffle_index]

y_train_5 = (y_train == 5)
y_test_5 = (y_test == 5)

sgd_clf = SGDClassifier(random_state=42)
sgd_clf.fit(X_train, y_train_5)

result=sgd_clf.predict([some_digit])

print("Predicting if #36000 is 5:")
print(result)

validation_result=cross_val_score(sgd_clf, X_train, y_train_5, cv=3, scoring="accuracy")

print("Cross Validation Result:")
print(validation_result)

y_train_predict = cross_val_predict(sgd_clf, X_train, y_train_5, cv=3)

matrix = confusion_matrix(y_train_5, y_train_predict)

print("Confusion matrix:\n")
print(matrix)

