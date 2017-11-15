from sklearn.datasets import fetch_mldata
import matplotlib
import matplotlib.pyplot as plt
import numpy as np
from sklearn.linear_model import SGDClassifier
from sklearn.model_selection import cross_val_score
from sklearn.model_selection import cross_val_predict
from sklearn.metrics import confusion_matrix
from sklearn.metrics import precision_score, recall_score
from sklearn.metrics import f1_score
from sklearn.metrics import precision_recall_curve
from sklearn.metrics import roc_curve
from sklearn.metrics import roc_auc_score
from sklearn.ensemble import RandomForestClassifier
from sklearn.multiclass import OneVsOneClassifier
from sklearn.preprocessing import StandardScaler

#Supress Panda FutureWarnings

import warnings
warnings.simplefilter(action='ignore', category=FutureWarning)

# Load the MNIST data set

mnist = fetch_mldata('MNIST original')
X, y = mnist["data"], mnist["target"]

# Display a digit (#36000)

some_digit = X[36000]
some_digit_image = some_digit.reshape(28, 28)
plt.imshow(some_digit_image, cmap = matplotlib.cm.binary, interpolation = "nearest")
plt.show()

# Split and shuffle data set

X_train, X_test, y_train, y_test = X[:60000], X[60000:], y[:60000], y[60000:]
shuffle_index = np.random.permutation(60000)
X_train, y_train = X_train[shuffle_index], y_train[shuffle_index]


# Train a binary classifier

y_train_5 = (y_train == 5)
y_test_5 = (y_test == 5)

sgd_clf = SGDClassifier(random_state=42)
sgd_clf.fit(X_train, y_train_5)

# Make a classification

result = sgd_clf.predict([some_digit])
print("Classification of #36000")
print(result)

# Evaluate accuracy

evaluation = cross_val_score(sgd_clf, X_train, y_train_5, cv=3, scoring="accuracy")
print("Accuracy evaluation")
print(evaluation)

# Calculate confusion matrix

y_train_pred = cross_val_predict(sgd_clf, X_train, y_train_5, cv=3)
matrix = confusion_matrix(y_train_5, y_train_pred)

print("Confusion matrix")
print(matrix)

# Precision and recall

print(precision_score(y_train_5, y_train_pred))
print(recall_score(y_train_5, y_train_pred))

# Calculate F1 score (harmonic mean of precision and recall)

print(f1_score(y_train_5, y_train_pred))

# recall/precision curve

y_scores = cross_val_predict(sgd_clf, X_train, y_train_5, cv=3, method = "decision_function")

precisions, recalls, thresholds = precision_recall_curve(y_train_5, y_scores)

def plot_precision_recall_threshold(precisions, recalls, thresholds):
	plt.plot(thresholds, precisions[:-1], "b--", label = "Precision")
	plt.plot(thresholds, recalls[:-1], "g-", label = "Recall")
	plt.xlabel("Threshold")
	plt.legend(loc="upper left")
	plt.ylim([0,1])

plot_precision_recall_threshold(precisions, recalls, thresholds)
plt.show()

y_train_pred_90 = (y_scores > 70000)
print(precision_score(y_train_5, y_train_pred_90))
print(recall_score(y_train_5, y_train_pred_90))

# plot ROC curve

fpr, tpr, thresholds = roc_curve(y_train_5, y_scores)

def plot_roc_curve(fpr, tpr, label=None):
	plt.plot(fpr, tpr, linewidth=2, label=label)
	plt.plot([0,1], [0,1], 'k--')
	plt.axis([0,1,0,1])
	plt.xlabel('False Positive Rate')
	plt.ylabel('True Positive Rate')

plot_roc_curve(fpr, tpr)
plt.show()
print(roc_auc_score(y_train_5, y_scores))

# Random forest classifier

forest_clf = RandomForestClassifier(random_state=42)
y_probas_forest = cross_val_predict(forest_clf, X_train, y_train_5, cv=3, method="predict_proba")
y_scores_forest = y_probas_forest[:, 1]
fpr_forest, tpr_forest, thresholds_forest = roc_curve(y_train_5, y_scores_forest)
plt.plot(fpr, tpr, "b:", label="SGD")
plot_roc_curve(fpr_forest, tpr_forest, "Random Forest")
plt.legend(loc = "lower right")
plt.show()
print(roc_auc_score(y_train_5, y_scores_forest))

#Multiclass classification

sgd_clf.fit(X_train, y_train)
some_digit_scores = sgd_clf.decision_function([some_digit])
print(some_digit_scores)

# One vs. One classifier

scaler = StandardScaler()
X_train_scaled = scaler.fit_transform(X_train.astype(np.float64))
ovo_clf = OneVsOneClassifier(SGDClassifier(random_state=42))
ovo_clf.fit(X_train, y_train)
print(cross_val_score(sgd_clf, X_train_scaled, y_train, cv=3, scoring="accuracy"))
y_train_pred = cross_val_predict(sgd_clf, X_train_scaled, y_train, cv=3)
conf_mx = confusion_matrix(y_train, y_train_pred)
plt.matshow(conf_mx, cmap=plt.cm.gray)
plt.show()

row_sums = conf_mx.sum(axis=1, keepdims=True)
norm_conf_mx = conf_mx / row_sums

np.fill_diagonal(norm_conf_mx, 0)
plt.matshow(norm_conf_mx, cmap=plt.cm.gray)
plt.show()

def plot_digits(instances, images_per_row=10, **options):
    size = 28
    images_per_row = min(len(instances), images_per_row)
    images = [instance.reshape(size,size) for instance in instances]
    n_rows = (len(instances) - 1) // images_per_row + 1
    row_images = []
    n_empty = n_rows * images_per_row - len(instances)
    images.append(np.zeros((size, size * n_empty)))
    for row in range(n_rows):
        rimages = images[row * images_per_row : (row + 1) * images_per_row]
        row_images.append(np.concatenate(rimages, axis=1))
    image = np.concatenate(row_images, axis=0)
    plt.imshow(image, cmap = matplotlib.cm.binary, **options)
    plt.axis("off")

cl_a, cl_b = 3, 5
X_aa = X_train[(y_train == cl_a) & (y_train_pred == cl_a)]
X_ab = X_train[(y_train == cl_a) & (y_train_pred == cl_b)]
X_ba = X_train[(y_train == cl_b) & (y_train_pred == cl_a)]
X_bb = X_train[(y_train == cl_b) & (y_train_pred == cl_b)]

plt.figure(figsize=(8,8))
plt.subplot(221); plot_digits(X_aa[:25], images_per_row=5)
plt.subplot(222); plot_digits(X_ab[:25], images_per_row=5)
plt.subplot(223); plot_digits(X_ba[:25], images_per_row=5)
plt.subplot(224); plot_digits(X_bb[:25], images_per_row=5)
plt.show()
