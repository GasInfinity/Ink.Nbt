## 🏷️ Ink.Nbt

Simple, JSON-like and high-performance NBT serialization library, inspired by the API of System.Text.Json.

[![MIT License](https://img.shields.io/badge/License-MIT-green.svg)](https://choosealicense.com/licenses/mit/) [![NuGet Version](https://img.shields.io/nuget/v/Ink.Nbt)](https://www.nuget.org/packages/Ink.Nbt/)
  
## 🔍 Usage

- Serializing and deserializing (Only handwritten converters support at this moment for AOT):
```cs
using Ink.Nbt;
using Ink.Nbt.Serialization;
using Ink.Nbt.Tags;
using System.Diagnostics;

NbtTag tag = NbtTag.Compound(new()
{
    { "SByte?", NbtTag.SByte(-1) },
    { "String?", NbtTag.String("Yes") }
});

{
    using FileStream file = File.OpenWrite("nbt.nbt.nbt");
    NbtSerializer.Serialize<JavaNbtDatatypeWriter, NbtTag>(file, "Too much nbt", tag, NbtTag.TypeInfo);
}

// ...

byte[] nbtNbtNbt = File.ReadAllBytes("nbt.nbt.nbt");
NbtTag tagAgain = NbtSerializer.Deserialize<JavaNbtDatatypeReader, NbtTag>(nbtNbtNbt, NbtTag.TypeInfo);

if(tagAgain["SByte?"].AsInt8() != -1 || tagAgain["String?"].AsString() != "Yes")
    throw new UnreachableException();
```

- Writing raw nbt data:
```cs
using Ink.Nbt;

using FileStream file = File.OpenWrite("nbt.nbt.nbt");
using NbtWriter<JavaNbtDatatypeWriter> writer = new(file);
writer.WriteCompoundStart("Too much nbt");
    writer.WriteSByte("SByte?", -1);
    writer.WriteString("String?", "Yes");
writer.WriteCompoundEnd();
```

- Reading raw nbt data:
```cs
using Ink.Nbt;

byte[] rawData = [ 0x0A, 0x01, 0x00, 0x00, 0xFF, 0x00 ];
NbtReader<JavaNbtDatatypeReader> reader = new(rawData, new(NoRootName: true));
while(reader.Read()) { } // TokenType: StartCompound -> PropertyName (string.Empty) -> SByte (-1) -> EndCompound -> End (return false)
```

## ❓ FAQ

#### Why do readers and writers need a generic argument?

This library wants to achieve maximum performance while supporting every kind of nbt that exists. The generic argument allows specifying the kind of NBT to be read or written.

#### Does this handle decompression and compression of NBT?

No, this library is primarily intended for working with raw NBT data. You should decompress it before and compress it after, if you want it compressed.

## 📜 License

[MIT](https://choosealicense.com/licenses/mit/)


## 📝 TODO

- [ ] Allow skipping data while reading
- [ ] Proper exception messages and info
- [ ] Proper tests with NUnit
- [ ] Refine serialization
- [ ] Support continuation while reading?
