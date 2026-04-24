namespace Utils._Project.Utils
{
    public static class InputUtils
    {
        private const string KeyboardStr = "<Keyboard>/";
        private const string MouseStr = "<Mouse>/";
        private const string GamepadStr = "<Gamepad>/";

        public static string PrettifyBinding(string bindingPath)
        {
            if (bindingPath.Contains(KeyboardStr))
            {
                bindingPath = bindingPath
                    .Replace(KeyboardStr, "");
            }
            else if (bindingPath.Contains(MouseStr))
            {
                bindingPath = bindingPath
                    .Replace(MouseStr, "");
            }
            else if (bindingPath.Contains(GamepadStr))
            {
                bindingPath = bindingPath
                    .Replace(GamepadStr, "");
            }

            return bindingPath.Replace(";", string.Empty);
        }
    }
}
