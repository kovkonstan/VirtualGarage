using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualGarage.Logic.Enums
{
    /// <summary>
    /// Результат авторизации пользователя
    /// </summary>
    public enum LoginResult
    {
		None = 0,
        Success,
        UserNotExist,
        UncorrectPass,
        Fail
    }
}
