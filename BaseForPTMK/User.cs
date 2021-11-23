using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseForPTMK
{
    public class User
    {
        /// <summary>
        /// Id пользователя
        /// </summary>
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// ФИО пользователя
        /// </summary>
        [Column("nameLastNameePatronymic")]
        public string FullName { get; set; }

         /// <summary>
         /// Дата рождения пользователя
         /// </summary>
        [Column("dateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Пол пользователя
        /// </summary>
        [Column("gender")]
        public string Gender { get; set; }

        public const string MaleGender = "мужской";
        public const string FemaleGedner = "женский";

    }
}
