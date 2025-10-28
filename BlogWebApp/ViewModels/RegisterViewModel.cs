﻿using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Имя пользователя обязателено.")]
        [MaxLength(50, ErrorMessage = "Имя пользователя не должно быть длиннее 50 символов.")]
        [RegularExpression("^[a-zA-Z0-9_-]{3,50}$",
            ErrorMessage = "Имя пользователя может содержать только латинские буквы, цифры, подчёркивания и дефисы.")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Email обязателен.")]
        [EmailAddress(ErrorMessage = "Некорректный email.")]
        [MaxLength(255, ErrorMessage = "Длина email не должна превышать 255 символов.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Пароль обязателен.")]
        [DataType(DataType.Password, ErrorMessage = "Некорректный пароль.")]
        [RegularExpression("^(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$",
            ErrorMessage = "Пароль должен содержать минимум 8 символов, хотя бы одну заглавную букву, цифру и спецсимвол.")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Подтвердите пароль.")]
        [DataType(DataType.Password, ErrorMessage = "Некорректный пароль.")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
