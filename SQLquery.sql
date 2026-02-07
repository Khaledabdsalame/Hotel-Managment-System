CREATE DATABASE Hotel_Management_System;

CREATE TABLE Users (
   user_id INT IDENTITY (1,1) NOT NULL,
   username VARCHAR(150) NOT NULL,
   User_Password VARCHAR(150) NOT NULL,
   CONSTRAINT User_Table_ID_PK PRIMARY KEY (user_id)
);

INSERT INTO Users (username, User_Password) VALUES 
    ('admin', 'admin123');



