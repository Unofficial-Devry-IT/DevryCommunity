using System.IO;

// Creates file at -- Project/bin/<framework>
StreamWriter writer = new StreamWriter("config.cfg");
writer.WriteLine("Version=1.0.0");
writer.WriteLine("Name=Mr. Bigglesworth");
writer.Close(); // Cannot forget to close this. Otherwise it'll be considered open by another program