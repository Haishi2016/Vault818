# Java Ingridients - String Operations

## Search for a pattern in a string
Use **indexOf** method of Java String class. The method returns the index within the target string of the first occurrence of the specified pattern, starting at the specified index. If the pattern is not found, the method returns **-1**.

```java
String text = "this is some text";
String pattern = "some";

int index = text.indexOf(pattern);
```
The **indexOf()** method is case-sensitive. If you try to search for "Some" in the preceding example, you'll get -1 because "Some" doesn't match with "some". If you want to ignore case, you can use **toLowerCase()** to convert both strings to all lower case letters and then do the search:

```java
String text = "this is some text";
String pattern = "some";

int index = text.toLowerCase().indexOf(pattern.toLowerCase());
```
## Search for a whole word
The preceding examples don't recognize word boundaries. If you search for "cat" in a sentence "I like to eat catfish", you'll get a match because the "cat" pattern does appear in the sentence. If you want to search for a whole word only, you can use one of the following solutions.

### Split the string
You can split the target string by spaces into a string array, and then check if the word matches with any of the array element. For example,

```java
String[] parts = "this is some text".split(“ “);
```

This gives you a string array with 4 individual words: “this”, “is”, “some” and “text”. Then, you can loop through the array and search for the word:

```java
private static int searchWord(String text, String word) {
    String[] parts = text.split(" ");
	for (int i = 0; i < parts.length; i++) {
		if (parts[i].equals(word)) {
			return 1;
		}
	}
	return -1;
}
```
## Use regular experssions
[Regular expression](https://en.wikipedia.org/wiki/Regular_expression) is a sequence that represents a search pattern. It has its own syntax that describes different components in a string. For example, “\b” represents a word boundary, which matches to any “non-word” characters. The following code uses regular expression to search for whole word, ignoring cases. It’s a bit complex but it handles many cases that aren’t handled by the previous implementation. For example, the previous example won’t work for “java=awesome” because it splits sentences only by spaces. The following example will work, because “=” is recognized as a word boundary as well.

```java
import java.util.regex.*;
...
private static int searchWord(String text, String word) {
	Pattern pattern = Pattern.compile("\\b" + word + "\\b", Pattern.CASE_INSENSITIVE);
	Matcher matcher = pattern.matcher(text);
	if (matcher.find()) {
		return matcher.start();
	}
	return -1;
}
```