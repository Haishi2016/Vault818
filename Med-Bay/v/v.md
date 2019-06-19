# V

## Vector

A vector is a list of numbers. The _dimension_ of a vector is the number of numbers in the list. THe numbers that make upa vector are called _entries. When a vector is written virtically, it's called a _column vector_ or _kets_. When a vector is written horizontally, it's called a _row vector_ or _bras_.

Examples: 
$\lvert v\rang = \begin{bmatrix}
   1 \\
   0.5\\
   -2
\end{bmatrix}$, 
$\lang w \lvert = \begin{bmatrix}
   1 & 0.5 & -2
\end{bmatrix}$

### Length

_Length_ of a vector $\lvert v\rang$ is defined as:

If $\lvert v\rang=\begin{bmatrix}
a_1 \\
a_2 \\
... \\
a_n
\end{bmatrix}$, then $\lvert\lvert v\rang\lvert=\sqrt{a_1^2+a_2^2+...+a_n^2}$.

### Scalar multiplication 
A ket $\lvert a\rang$ multiplied by number _c_:

If $\lvert a\rang=\begin{bmatrix}
a_1 \\
a_2 \\
... \\
a_n
\end{bmatrix}$, then $c\cdot\lvert v\rang=\begin{bmatrix}
c\cdot a_1 \\
c\cdot a_2 \\
... \\
c\cdot a_n
\end{bmatrix}$.

### Vector addition

Addition of vectors:

If $\lvert a\rang=\begin{bmatrix}
a_1 \\
a_2 \\
... \\
a_n
\end{bmatrix}$, $\lvert b\rang=\begin{bmatrix}
b_1 \\
b_2 \\
... \\
b_n
\end{bmatrix}$, then $\lvert a+b\rang=\lvert b+a\rang=\begin{bmatrix}
a_1 + b_1 \\
a_2 + b_2 \\
... \\
a_n + b_n
\end{bmatrix}$.

### Inner proudct (dot product)

If $\lang a \lvert=\begin{bmatrix}
a_1 & a_2 & ... a_n
\end{bmatrix}$, $\lvert b \rang=\begin{bmatrix}
b_1 \\
b_2 \\
... \\
b_n
\end{bmatrix}$, then $\lang a\lvert b\rang=a_1b_1+a_2b_2+...+a_nb_n$.

Therefor, $\lvert\lvert a\rang\lvert=\sqrt{\lang a\lvert a\rang}$.

### Orthogonality

Two kets $\lvert a\rang$ and $\lvert b\rang$ are orthogonal if and only if $\lang a\lvert b\rang=0$.

### Orthonomal Bases

For _n_-dimensional kets, an orthonormal basis consists of a set of _n_ unit kets that are mutually orthogonal to one another.

We define:

$\lvert\uparrow\rang=\begin{bmatrix}1\\0\end{bmatrix}$,
$\quad\lvert\downarrow\rang=\begin{bmatrix}0\\1\end{bmatrix}$,
$\quad\lvert\rightarrow\rang=\begin{bmatrix}\frac{1}{\sqrt{2}}\\\frac{-1}{\sqrt{2}}\end{bmatrix}$,
$\quad\lvert\leftarrow\rang=\begin{bmatrix}\frac{1}{\sqrt{2}}\\\frac{1}{\sqrt{2}}\end{bmatrix}$,
$\quad\lvert\nearrow\rang=\begin{bmatrix}\frac{1}{2}\\\frac{-\sqrt{3}}{\sqrt{2}}\end{bmatrix}$,
$\quad\lvert\swarrow\rang=\begin{bmatrix}\frac{\sqrt{3}}{2}\\\frac{1}{2}\end{bmatrix}$.

Three bases:

$\lbrace\lvert\uparrow\rang,\lvert\downarrow\rang\rbrace$,
$\quad\lbrace\lvert\rightarrow\rang,\lvert\leftarrow\rang\rbrace$,
$\quad\lbrace\lvert\nearrow\rang,\lvert\swarrow\rang\rbrace$.

___

## References

- Quantum Computing for Everyone, Chris Bernhardt 2019.