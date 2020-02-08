namespace QuizHut.Services.Common
{
    public static class Base64
    {
        public static string Encode(string id)
        {
            var idBytes = System.Text.Encoding.UTF8.GetBytes(id);
            return System.Convert.ToBase64String(idBytes);
        }

        public static string Decode(string encodedId)
        {
            var encodedIdBytes = System.Convert.FromBase64String(encodedId);
            return System.Text.Encoding.UTF8.GetString(encodedIdBytes);
        }
    }
}
