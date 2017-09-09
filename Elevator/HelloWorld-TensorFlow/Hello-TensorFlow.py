import tensorflow as tf 

node = tf.constant("Hello, World!")
session = tf.Session()

print(session.run(node))