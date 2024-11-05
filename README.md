# Serialisation

This is a library used to handle the serialization and deserialization of instanced classes
developed for my own future use.  

## Serialization

SerializationManager.Serialize() contains overloads for either writing the serialized bytes directly to disk or it can also return the byte array directly.

## Deserialization

SerializationManager.Deserialize() contains overloads for either loading an object from a file location or from a byte array.
Said object will be internally consistent with the one that got serialized.
SerializationManager.Deserialize() also accept generics.

## How to set up a class

Mark a class that you want to be able to (de)serialize with ClassSerializationAttribute and any properties you want (de)serialized with PropertySerializationAttribute.

### ClassSerializationAttribute

ClassSerializationAttribute does 2 things, first it tells the SerializationManager that this class is intended to be serialized and it also provides and ID which the deserializer uses to find the correct type.
Keep the ID unique and greater than 0.  
0 and the negatives are reserved for special cases.

### PropertySerializationAttribute

So now the (de)serializer knows that the class is safe to (de)serialize it still has the problem of not knowing which properties it should touch.  
This is where PropertySerializationAttribute comes in, the (de)serializer will (de)serialize any properties with the PropertySerializationAttribute.  
The name property of the attribute is used by the deserializer to know what property it is currently deserializing so keep these unique within a single class.
Two different classes can both have attributes with the same name without any issues but a single class cannot have 2 attributes the same name.

## ISerialization

ISerialization is an interface used by both the serializer and the deserializer quite heavily. It is primarily used for properties but it will also be attempted to be used for classes that miss a ClassSerializationAttribute.  
When either SerializationManager.Serialize() or SerializationManager.Deserialize() are called for the first time every class with the ISerialization interface are loaded into a dictionary to be used by the (de)serializer.
So if you want to make your own it will be used as long as it is in the domain when either of those methods are called for the first time.
Priority will also be given to external classes, so you can overwrite something like StringSerializer if you want and the code will go with your version.

### ISerialization.SerializationType

SerializationType just the type that this class will be used to (de)serialize.

### ISerialization.Serialize

This method takes an object, the object will be of the ISerialization.SerializationType.
The return value will be a byte array that can be used by the deserializer to reconstruct the object that was passed to the method.

### ISerialization.Deserialize

This method will recieve the same byte array that ISerialization.Serialize returned.
It should return a functionally identical object, but if it's your implementation you can do whatever you want.

## Support

As of writing this SerializationManager has support for all basic data types with the exception of object, nint and nuint.
It can also has implicit array support for any type it can serialize I. E. anything that has either a ClassSerializationAttribute or an ISerializer. This includes both multi-dimensional arrays and jagged arrays.
