# Serialisation

This is a library to used handle the serialization and deserialization of instanced classes
developed for my own future use.  

## Serialization

So there are two methods as of writing this to serialize an instance of an object and those are SerializationManager.Serialize() and SerializationManager.SaveToFile().  
Serialize() gives you back a byte array and SaveToFile() does the same thing but it just saves it to a specificed location instead of giving you the bytes.

## Deserialization

With deserialization there are a few more options compared to serialization. There are two main methods and those Deserialize() and LoadObjectFromFile().  
Deserialize takes the byte array you got from Serialize() and LoadObjectFromFile() reads bytes from a file. They both give you back an object that will be mostly identical to what you put in with the exception of references.  
Because of either technical limitations or my own incompetence any references will be copied so if you have to properties pointing to the same instance on the heap before serialization, after deserialization they will now be seperate objects.  

Both Deserialization methods also accept generics that will just do a cast for you.  

## How to set up a class

If you throw any object into Serialize() you will most likely get an exception. That is because it will only try and serialize classes with a ClassSerializationAttribute or if they have ISerialization interface.  

### ClassSerializationAttribute

ClassSerializationAttribute does 2 things, first it tells the SerializationManager that this class is intended to be serialized and it also provides and ID which the deserializer uses to find the correct type.
Keep the ID unique and greater than 0.  
0 and the negatives are reserved for special cases.

### PropertySerializationAttribute

So now the (De)serializer knows that the class is safe to (De)serialize it still has the problem of not knowing which properties it should touch.  
This is where PropertySerializationAttribute comes in, the (De)serializer will (De)serialize any properties with the PropertySerializationAttribute.  
The name property of the attribute is used by the deserializer to know what property it is currently deserializing so keep these unique within a single class.
Two different classes can both have attributes with the same name without any issues but a single class cannot have 2 attributes the same name.

## ISerialization

ISerialization is an interface used by both the serializer and the deserializer quite heavily. It is primarily used for properties but it will also be attempted to be used for classes that miss a ClassSerializationAttribute.  
When either SerializationManager.Serialize() or SerializationManager.Deserialize() are called for the first time every class with the ISerialization interface are loaded into a dictionary to be used by the (de)serializer.
So if you want to make your own it will be used as long as it is in the domain when either of those methods are called for the first time.

### ISerialization.SerializationType

SerializationType just the type that this class will be used to (De)serialize.

### ISerialization.Serialize

This method takes an object, the object will be of the ISerialization.SerializationType.
The return value will be a byte array that can be used by the deserializer to reconstruct the object that was passed to the method.

### ISerialization.Deserialize

This method will recieve the same byte array that ISerialization.Serialize returned.
It should return a functionally identical object, but if it's your implementation you can do whatever you want.

## Support

As of writing this SerializationManager has support for all basic data types with the exception of object, nint and nuint.
It can also has implicit array support for any type it can serialize I. E. anything that has either a ClassSerializationAttribute or an ISerializer. This includes both multi-dimensional arrays and jagged arrays.
