namespace Quantum.QuantumEraser
{
    open Microsoft.Quantum.Intrinsic;
    open Microsoft.Quantum.Canon;
    open Microsoft.Quantum.Math;

    operation HelloQ () : Result {
       using (q = Qubit()) {
			using (c = Qubit()) {
				let sita = PI()/10.0;
				H(q);	//put the qubit in the superposition of both paths/slits
				CNOT(q,c); //copy which-way information
				R1(sita*2.0, q);
				CNOT(q,c); //erasure of which-way information
				H(q);	//make the two paths interference
				AssertProb([PauliZ], [q], Zero, Cos(sita)*Cos(sita), "BAD", 1E-05 );	//P(0)=cos(pi/4)^2
				//AssertProb([PauliZ], [q], Zero, 0.5, "BAD", 1E-05 );	//P(0)=cos(pi/4)^2


				let result = Measure([PauliZ],[q]);
				if (result == One) {
					X(q);
				}
				let result2 = Measure([PauliZ],[c]);
				if (result2 == One) {
					X(c);
				}
				return result;
			}
		}
    }
}
