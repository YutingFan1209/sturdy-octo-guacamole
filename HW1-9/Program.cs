using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

/*
01 Introduction to C# and Data Types - Short Answers

What type would you choose for the following "numbers"?

1. A person's telephone number:
   string, because phone numbers are identifiers, not values used for math.
   They can include leading zeros, spaces, parentheses, plus signs, or dashes.

2. A person's height:
   double or decimal. double is common for measurements; decimal is useful if
   exact decimal precision is required.

3. A person's age:
   byte, short, or int. int is most common because it is simple and standard.

4. A person's gender (Male, Female, Prefer Not To Answer):
   enum, for example Gender.Male, Gender.Female, Gender.PreferNotToAnswer.

5. A person's salary:
   decimal, because money should avoid floating-point rounding errors.

6. A book's ISBN:
   string, because ISBNs can have leading zeros and hyphens and are not used
   for arithmetic.

7. A book's price:
   decimal, because it represents money.

8. A book's shipping weight:
   double or decimal. double is common for measurements; decimal is useful for
   exact decimal values.

9. A country's population:
   int may work for many countries, but long is safer for very large values.

10. The number of stars in the universe:
    double, because this is an extremely large estimated/scientific value.

11. Number of employees in each small or medium business in the UK:
    ushort or int. Since the max is about 50,000, ushort works, but int is more
    commonly used in C#.

Value types vs reference types:
Value type variables store their value directly. Examples include int, double,
bool, decimal, structs, and enums. Reference type variables store a reference
to an object on the heap. Examples include string, arrays, classes, and List<T>.
Assigning a value type usually copies the value. Assigning a reference type
copies the reference, so two variables can point to the same object.

Boxing and unboxing:
Boxing converts a value type into object or an interface type, wrapping the
value on the heap. Unboxing converts the boxed object back to the value type.
Example: object boxed = 123; int number = (int)boxed;

Managed resources and unmanaged resources:
Managed resources are objects controlled by the .NET runtime, such as normal
C# objects, strings, arrays, and Lists. Unmanaged resources are outside direct
GC control, such as file handles, database connections, sockets, window handles,
and unmanaged memory.

Purpose of the Garbage Collector:
The Garbage Collector automatically frees memory used by managed objects that
are no longer reachable, helping prevent memory leaks and manual memory errors.


Controlling Flow and Converting Types - Short Answers

What happens when you divide an int variable by 0?
It throws a DivideByZeroException at runtime.

What happens when you divide a double variable by 0?
It does not throw. Positive values produce Infinity, negative values produce
-Infinity, and 0.0 / 0.0 produces NaN.

What happens when you overflow an int variable?
In an unchecked context, it wraps around. In a checked context, it throws an
OverflowException.

Difference between x = y++; and x = ++y:
x = y++ assigns y to x first, then increments y.
x = ++y increments y first, then assigns the new value to x.

Difference between break, continue, and return inside a loop:
break exits the loop. continue skips the rest of the current iteration and
continues with the next iteration. return exits the entire method.

Three parts of a for statement:
for (initializer; condition; iterator)
All three parts are optional, but the two semicolons are required.

Difference between = and ==:
= assigns a value. == compares two values for equality.

Does this compile? for ( ; true; ) ;
Yes. It compiles and creates an infinite loop with an empty body.

What interface must an object implement to be enumerated by foreach?
Usually IEnumerable or IEnumerable<T>. The object can also follow the foreach
pattern by providing a suitable GetEnumerator method.


Additional C# Short Answers

Purpose of the params keyword:
params lets a method accept a variable number of arguments as an array.
Example: static int Sum(params int[] numbers) lets you call Sum(1, 2, 3).

Nullable value types vs nullable reference types:
A nullable value type, like int?, can store either an int value or null.
Nullable reference types are a C# compiler feature that helps warn when a
reference may be null. They do not create a different runtime type.

New features in C# 7, 8, 9, and 10:
C# 7 introduced tuples, pattern matching, out variables, local functions, and
discards. C# 8 introduced nullable reference types, switch expressions, using
declarations, async streams, and indices/ranges. C# 9 introduced records,
init-only setters, top-level statements, and improved pattern matching. C# 10
introduced global using directives, file-scoped namespaces, record structs, and
improvements to lambdas and structs.

Garbage Collector generations:
Generation 0 contains short-lived objects and is collected most often.
Generation 1 is a buffer between short-lived and long-lived objects.
Generation 2 contains long-lived objects and is collected less often.
The generations exist to improve performance because most objects die young.

IDisposable:
IDisposable provides a Dispose() method for releasing resources deterministically.
A class should implement IDisposable when it owns unmanaged resources or owns
other IDisposable objects. Dispose does not depend on waiting for the Garbage
Collector; it lets code release resources immediately.

using statement with IDisposable:
The using statement or using declaration automatically calls Dispose() when the
object goes out of scope, even if an exception happens.

using statement vs using directive:
A using directive imports a namespace, for example using System.Text;.
A using statement/declaration manages the lifetime of an IDisposable object.

.NET Framework vs .NET Core / modern .NET:
.NET Framework is the older Windows-only framework. .NET Core and modern .NET
are cross-platform, open-source, faster moving, and used for current development.

string vs StringBuilder:
string is immutable, so changes create new string objects. StringBuilder is
mutable and is better when building or modifying strings many times in a loop.

var vs dynamic vs explicit typing:
var is statically typed; the compiler infers the type at compile time.
dynamic is resolved at runtime and skips compile-time type checking.
Explicit typing names the type directly, such as int count = 5;.

ref, out, and in parameters:
ref passes a variable by reference and must be assigned before the call.
out passes by reference for output and must be assigned inside the method.
in passes by readonly reference and prevents the method from modifying it.

Access modifiers:
public is accessible everywhere.
private is accessible only inside the containing type.
protected is accessible inside the containing type and derived types.
internal is accessible inside the same assembly.
protected internal is accessible from the same assembly or derived types.
private protected is accessible from derived types in the same assembly.

Method overloading vs overriding:
Overloading means multiple methods have the same name but different parameter
lists. Overriding means a derived class provides a new implementation of a
virtual, abstract, or override method from a base class.

Array vs List<T>:
An array has a fixed size and is useful when the number of elements is known.
List<T> can grow and shrink and is easier when the number of elements changes.
*/

public enum Gender
{
    Male,
    Female,
    PreferNotToAnswer
}

public class Program
{
    public static void Main()
    {
        Console.WriteLine("C# homework answers are in Program.cs.");
        Console.WriteLine("Uncomment sample calls in Main if you want to run the coding methods.");

        // CodingQuestion1.PrintTypeInfo();
        // CodingQuestion2.FizzBuzz(30);
        // CodingQuestion3.ExplainByteLoop();
        // int[] answer = CodingQuestion4.TwoSum(new[] { 2, 7, 11, 15 }, 9);
        // Console.WriteLine($"Two Sum indices: [{answer[0]}, {answer[1]}]");
        // Console.WriteLine(ParamsExample.Sum(1, 2, 3, 4, 5));
    }
}

public class CodingQuestion1
{
    public static void PrintTypeInfo()
    {
        Print("sbyte", sbyte.MinValue, sbyte.MaxValue, Marshal.SizeOf<sbyte>());
        Print("byte", byte.MinValue, byte.MaxValue, Marshal.SizeOf<byte>());
        Print("short", short.MinValue, short.MaxValue, Marshal.SizeOf<short>());
        Print("ushort", ushort.MinValue, ushort.MaxValue, Marshal.SizeOf<ushort>());
        Print("int", int.MinValue, int.MaxValue, Marshal.SizeOf<int>());
        Print("uint", uint.MinValue, uint.MaxValue, Marshal.SizeOf<uint>());
        Print("long", long.MinValue, long.MaxValue, Marshal.SizeOf<long>());
        Print("ulong", ulong.MinValue, ulong.MaxValue, Marshal.SizeOf<ulong>());
        Print("float", float.MinValue, float.MaxValue, Marshal.SizeOf<float>());
        Print("double", double.MinValue, double.MaxValue, Marshal.SizeOf<double>());
        Print("decimal", decimal.MinValue, decimal.MaxValue, Marshal.SizeOf<decimal>());
    }

    private static void Print<T>(string typeName, T minValue, T maxValue, int bytes)
    {
        Console.WriteLine($"{typeName,-8} Min: {minValue} Max: {maxValue} Bytes: {bytes}");
    }
}

public class CodingQuestion2
{
    public static void FizzBuzz(int num)
    {
        for (int i = 1; i <= num; i++)
        {
            if (i % 3 == 0 && i % 5 == 0)
            {
                Console.WriteLine("FizzBuzz");
            }
            else if (i % 3 == 0)
            {
                Console.WriteLine("Fizz");
            }
            else if (i % 5 == 0)
            {
                Console.WriteLine("Buzz");
            }
            else
            {
                Console.WriteLine(i);
            }
        }
    }
}

public class CodingQuestion3
{
    public static void ExplainByteLoop()
    {
        /*
        Code:

        int max = 500;
        for (byte i = 0; i < max; i++)
        {
            Console.WriteLine(i);
        }

        Explanation:
        byte can only store values from 0 to 255. Since max is 500, the
        condition i < max is always true for every possible byte value.
        When i reaches 255 and increments, it overflows back to 0 in the
        default unchecked context. The loop becomes infinite and prints
        0 through 255 repeatedly.

        In a checked context, incrementing past 255 would throw an
        OverflowException instead.
        */
        Console.WriteLine("The byte loop becomes infinite in an unchecked context.");
    }
}

public class CodingQuestion4
{
    public static int[] TwoSum(int[] nums, int target)
    {
        Dictionary<int, int> seen = new Dictionary<int, int>();

        for (int i = 0; i < nums.Length; i++)
        {
            int needed = target - nums[i];

            if (seen.ContainsKey(needed))
            {
                return new[] { seen[needed], i };
            }

            if (!seen.ContainsKey(nums[i]))
            {
                seen.Add(nums[i], i);
            }
        }

        throw new ArgumentException("No two sum solution exists.");
    }
}
