# FAQ

## MYSQL Stored Proc Commands incorrectly parsed

For MySQL or MariaDB, if your SQL statement is constructing a stored procedure, the parser may incorrectly split the commands.
For example, given the following:

```sql
CREATE PROCEDURE My_StoreProc(strEmail VARCHAR(255))
BEGIN
SELECT id FROM table_name WHERE email = strEmail
ORDER BY id DESC limit 1;
END;
```

The parser may incorrectly create 2 commands from this, which will fail on execution.
MySql dialect has the option to change the delimiter for cases like this.  So for this example, changing the delimiter to `$$` will get the parser to correclty split the command:

```sql
DELIMITER $$

CREATE PROCEDURE My_StoreProc(strEmail VARCHAR(255))
BEGIN
SELECT id FROM table_name WHERE email = strEmail
ORDER BY id DESC limit 1;
END$$
```

See the MySQL [docs](https://dev.mysql.com/doc/refman/8.0/en/stored-programs-defining.html) for further info.


