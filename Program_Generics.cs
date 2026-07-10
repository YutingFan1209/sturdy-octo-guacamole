using System;
using System.Collections.Generic;
using System.Reflection;

/*
Generics Questions

Describe the problem generics address.
Generics let us write reusable, type-safe code without rewriting the same class
or method for every data type. They also avoid unnecessary casting and reduce
runtime errors compared with using object everywhere.

How would you create a list of strings, using the generic List class?
List<string> names = new List<string>();

How many generic type parameters does the Dictionary class have?
Dictionary<TKey, TValue> has two generic type parameters: one for the key type
and one for the value type.

True/False. When a generic class has multiple type parameters, they must all match.
False. Multiple type parameters can be different types, like Dictionary<int, string>.

What method is used to add items to a List object?
Add().

Name two methods that cause items to be removed from a List.
Remove() and RemoveAt(). Other examples include RemoveAll(), RemoveRange(), and Clear().

How do you indicate that a class has a generic type parameter?
Put the type parameter in angle brackets after the class name.
Example: public class Box<T>

True/False. Generic classes can only have one generic type parameter.
False. Generic classes can have multiple type parameters.

True/False. Generic type constraints limit what can be used for the generic type.
True.

True/False. Constraints let you use the methods of the thing you are constraining to.
True. For example, where T : SomeBaseClass lets the generic code use members
defined by SomeBaseClass.
*/

public class Program
{
    public static void Main()
    {
        PracticeTask1.Run();
        PracticeTask2.Run();
    }
}

public class PracticeTask1
{
    public static void Run()
    {
        MyStack<int> numbers = new MyStack<int>();

        numbers.Push(10);
        numbers.Push(20);

        Console.WriteLine($"Current number of elements in MyStack<int>: {numbers.Count()}");
    }
}

public class MyStack<T>
{
    private readonly Stack<T> items = new Stack<T>();

    public int Count()
    {
        return items.Count;
    }

    public T Pop()
    {
        return items.Pop();
    }

    public void Push(T obj)
    {
        items.Push(obj);
    }
}

public class PracticeTask2
{
    public static void Run()
    {
        IGenericRepository<Product> repository = new GenericRepository<Product>();

        Product keyboard = new Product
        {
            Id = 1,
            Name = "Keyboard"
        };

        Product mouse = new Product
        {
            Id = 2,
            Name = "Mouse"
        };

        repository.Add(keyboard);
        repository.Add(mouse);

        Product foundProduct = repository.GetById(2);

        Console.WriteLine($"Repository item count: {CountItems(repository.GetAll())}");
        Console.WriteLine($"Found product: {foundProduct.Name}");

        repository.Remove(keyboard);
        repository.Save();
    }

    private static int CountItems<T>(IEnumerable<T> items)
    {
        int count = 0;

        foreach (T item in items)
        {
            count++;
        }

        return count;
    }
}

public interface IGenericRepository<T> where T : class
{
    void Add(T item);

    void Remove(T item);

    void Save();

    IEnumerable<T> GetAll();

    T GetById(int id);
}

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly List<T> data;

    public GenericRepository()
    {
        data = new List<T>();
    }

    public void Add(T item)
    {
        data.Add(item);
    }

    public void Remove(T item)
    {
        data.Remove(item);
    }

    public void Save()
    {
        // No actual implementation is needed for this exercise.
    }

    public IEnumerable<T> GetAll()
    {
        return new List<T>(data);
    }

    public T GetById(int id)
    {
        PropertyInfo? idProperty = typeof(T).GetProperty("Id");

        if (idProperty == null || idProperty.PropertyType != typeof(int))
        {
            throw new InvalidOperationException("Type T must have an int Id property to use GetById.");
        }

        foreach (T item in data)
        {
            object? value = idProperty.GetValue(item);

            if (value is int itemId && itemId == id)
            {
                return item;
            }
        }

        throw new KeyNotFoundException($"No item with Id {id} was found.");
    }
}

public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
