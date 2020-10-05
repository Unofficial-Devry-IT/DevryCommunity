using System.IO;

// Creates file at -- Project/bin/<framework>
// Since StreamWriter implements **IDisposable** we can use a using block!
// This automatically cleans up the object if used in a using block
using(StreamWriter writer = new StreamWriter("config.cfg"))
{
    writer.WriteLine("Version=1.0.0");
    writer.WriteLine("Name=Mr. Bigglesworth");
}
// once here the writer is automatically closed or "Disposed"
