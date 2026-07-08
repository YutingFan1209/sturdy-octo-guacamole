using System;
using System.Collections.Generic;

/*
OOP Q&A

What are the six combinations of access modifier keywords and what do they do?
1. public: Accessible from anywhere.
2. private: Accessible only inside the containing type.
3. protected: Accessible inside the containing type and derived types.
4. internal: Accessible only within the same assembly/project.
5. protected internal: Accessible from the same assembly or from derived types.
6. private protected: Accessible from derived types, but only in the same assembly.

What is the difference between static, const, and readonly keywords when applied to a type member?
static means the member belongs to the type itself instead of an object instance.
const means the value is a compile-time constant and cannot change.
readonly means the field can be assigned only when declared or inside a constructor.

What does a constructor do?
A constructor runs when an object is created. It initializes the object's fields
and properties so the object starts in a valid state.

Why is the partial keyword useful?
partial lets one class, struct, interface, or method be split across multiple
files. It is useful for generated code, large classes, and keeping code organized.

What is a tuple?
A tuple groups multiple values into one object without creating a custom class.
Example: (int id, string name) person = (1, "Ana");

What does the C# record keyword do?
record creates a reference type or value type designed for immutable data models.
Records provide value-based equality, concise syntax, and built-in support for
non-destructive mutation using with expressions.

What does overloading and overriding mean?
Overloading means creating multiple methods with the same name but different
parameter lists. Overriding means a derived class replaces a virtual or abstract
base class method with its own implementation.

What is the difference between a field and a property?
A field stores data directly. A property controls access to data, often wrapping
a field with get and set logic.

How do you make a method parameter optional?
Give the parameter a default value.
Example: void PrintName(string name = "Unknown")

What is an interface and how is it different from an abstract class?
An interface defines a contract that implementing classes must follow. An
abstract class can provide shared state, constructors, and implemented methods.
A class can implement multiple interfaces, but it can inherit from only one base
class.

What accessibility level are members of an interface by default?
Interface members are public by default.

True/False: Polymorphism allows derived classes to provide different implementations of the same method.
True.

True/False: The override keyword is used to indicate that a method in a derived class is providing its own implementation.
True.

True/False: The new keyword is used to indicate that a method in a derived class is providing its own implementation.
True, but it hides the base member instead of overriding it polymorphically.

True/False: Abstract methods can be used in a normal (non-abstract) class.
False. A class with an abstract method must be abstract.

True/False: Normal (non-abstract) methods can be used in an abstract class.
True.

True/False: Derived classes can override methods that were virtual in the base class.
True.

True/False: Derived classes can override methods that were abstract in the base class.
True.

True/False: Derived classes must override the abstract methods from the base class.
True, unless the derived class is also abstract.

True/False: In a derived class, you can override a method that was neither virtual nor abstract in the base class.
False.

True/False: A class that implements an interface does not have to provide an implementation for all of the members of the interface.
False, unless the class is abstract.

True/False: A class that implements an interface is allowed to have other members in addition to the interface members.
True.

True/False: A class can inherit from more than one base class.
False.

True/False: A class can implement more than one interface.
True.


Interview Questions

What are the four OOP principles in C#? Can you give examples of how you have applied them in a real project?
The four principles are encapsulation, inheritance, polymorphism, and abstraction.
In a real project, I might encapsulate salary validation inside a Salary property,
use inheritance for Person, Student, and Instructor, use polymorphism through a
virtual GetDescription method, and use abstraction through interfaces or abstract
base classes that define common behavior without exposing every detail.

What are virtual, abstract, and override methods? How do they work together?
virtual methods have a base implementation that derived classes may replace.
abstract methods have no implementation and must be implemented by non-abstract
derived classes. override is used in the derived class to provide the new
implementation.

What is polymorphism? Can you explain compile-time polymorphism and runtime polymorphism with examples?
Polymorphism means one operation can behave differently depending on the object
or parameters. Compile-time polymorphism is method overloading, such as Print(int)
and Print(string). Runtime polymorphism uses virtual/abstract methods and override,
where a base-class variable can call a derived-class implementation.

What is encapsulation, and how does it help make code easier to maintain?
Encapsulation hides internal details and exposes controlled public members. It
makes code easier to maintain because validation and business rules stay inside
the class instead of being repeated throughout the program.

What is inheritance? When would you avoid inheritance and use composition instead?
Inheritance lets one class reuse and extend another class. I would avoid
inheritance when the relationship is not truly an "is-a" relationship or when
composition would be more flexible. For example, a Car has an Engine, so
composition is better than making Car inherit from Engine.

What are access modifiers in C#?
public, private, protected, internal, protected internal, and private protected
control where a type or member can be accessed from.

What is the difference between fields, properties, and auto-properties?
Fields store data directly. Properties expose data through get and set accessors.
Auto-properties are shorthand properties where the compiler creates the backing
field automatically, such as public int Id { get; set; }.

What is the difference between static, sealed, and abstract? When would you use each keyword?
static means a member belongs to the type, or a class cannot be instantiated and
contains only static members. sealed prevents inheritance. abstract requires
inheritance because the type or member is incomplete. Use static for shared
helpers, sealed to prevent further inheritance, and abstract for base concepts.

What C# coding practices do you follow to write clean, maintainable, and testable code?
Use clear names, small methods, single responsibility, meaningful exceptions,
properties for validation, interfaces for dependencies, avoid duplicated logic,
write unit-testable methods, and keep classes focused.

What is the difference between a class, a struct, and a record? When would you use each?
A class is a reference type and is good for objects with identity and behavior.
A struct is a value type and is good for small immutable values. A record is
designed for data models with value-based equality and is useful for DTOs,
configuration objects, and immutable data.
*/

public abstract class Person

{

    private decimal salary;

    public int Id { get; set; }

    public decimal Salary

    {

        get

        {

            return salary;

        }

        set

        {

            if (value < 0)

            {

                throw new ArgumentException("Salary cannot be negative.");

            }

            salary = value;

        }

    }

    public DateTime DateOfBirth { get; set; }

    public List<string> Address { get; set; } = new List<string>();

}

public class Instructor : Person

{

    public int DepartmentId { get; set; }

}

public class Student : Person

{

    public List<Course> SelectedCourses { get; set; } = new List<Course>();

}

public class Course

{

    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

}