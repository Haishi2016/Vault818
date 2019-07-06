# C

## CNOT gate

Controlled NOT gate takes two inputs and generates two outputs. When the first input, which is called control bit, is 0, it has no effect on the second bit (the target bit). If the control bit is 1, the CNOT gate acts as a NOT gate on the target bit, as shown in the following table:

|x|y|x|x$\bigoplus$y|
|---|---|---|---|
| 0 | 0 | 0 | 0 |
| 0 | 1 | 0 | 1 |
| 1 | 0 | 1 | 1 |
| 1 | 1 | 1 | 0 |

In quantum computing, CNOT gate is often used to entangle two qubits.

$CNOT(r\vert00\rang+s\vert01\rang+t\vert10\rang+u\vert11\rang)=r\vert00\rang+s\vert01\rang+u\vert10\rang+t\vert11\rang$

___

## References

- Quantum Computing for Everyone, Chris Bernhardt 2019.