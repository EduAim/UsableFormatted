namespace UsableFormatted.Model
{
    public class SurveyAnswers
    {
        public int FontSize { get; set; }
        public int HeadingSize { get; set; }
        public int Survey1 { get; set; }
        public int Survey2 { get; set; }
        public int Survey3 { get; set; }
        public string FontName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }

    /*
CREATE TABLE `docproc_survey` (
`id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
`record_time` TIMESTAMP NOT NULL DEFAULT current_timestamp(),
`font_size` TINYINT(4) NOT NULL,
`heading_size` TINYINT(4) NOT NULL,
`survey_1` TINYINT(4) NOT NULL,
`survey_2` TINYINT(4) NOT NULL,
`survey_3` TINYINT(4) NOT NULL,
`font_name` VARCHAR(50) NOT NULL COLLATE 'utf8mb4_general_ci',
`user_name` VARCHAR(255) NOT NULL COLLATE 'utf8mb4_general_ci',
PRIMARY KEY (`id`) USING BTREE
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
;
    //*/
}