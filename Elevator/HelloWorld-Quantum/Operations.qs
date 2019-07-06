namespace Quantum.HelloWorld_Quantum
{
    open Microsoft.Quantum.Intrinsic;
    open Microsoft.Quantum.Canon;
	open Microsoft.Quantum.Math;
    
    operation HelloQ () : Result {
       
	   using (qubit = Qubit()) {
					
			H(qubit);
			AssertProb([PauliZ], [qubit], One, 0.5, "Error: Outcomes of the measurement must be equally likely", 1E-05);

			//Ry(ArcSin(1.0/Sqrt(3.0))*2.0, qubit);
			//AssertProb([PauliZ], [qubit], One, 1.0/3.0, "Error: Outcomes of 1s must be 33.3%", 1E-05);

				let result = Measure([PauliZ],[qubit]);
				if (result == One) {
					X(qubit);
				}
				return result;
		}        

    }
}
