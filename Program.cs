catch (Exception ex)
{
    var cs = builder.Configuration.GetConnectionString("DefaultConnection") ?? "NULL";
    var safe = System.Text.RegularExpressions.Regex.Replace(cs, "Password=[^;]*", "Password=***");
    File.WriteAllText(
        Path.Combine(AppContext.BaseDirectory, "startup-error.txt"),
        "CONNECTION USED: " + safe + "\n\n" + ex.ToString());
