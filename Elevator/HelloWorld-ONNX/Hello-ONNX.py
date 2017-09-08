from onnx.onnx_pb2 import *
from onnx import checker, helper

graph_proto = helper.make_graph(
        [
                helper.make_node("FC", ["X", "W1", "B1"], ["A1"]),
                helper.make_node("Sigmoid", ["A1", "W2", "B2"], ["A2"]),
                helper.make_node("FC", ["A2", "W3", "B3"], ["Y"])
        ],
        "Hello World!",
        ["X", "W1", "B1", "W2", "B2", "W3", "B3"],
        ["Y"]
)

print(str(graph_proto.name))