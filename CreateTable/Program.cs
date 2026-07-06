using System;
using Oracle.ManagedDataAccess.Client;

class Program
{
    static void Main()
    {
        string connStr = "Data Source=10.50.1.62/orcl;User ID=bbhrm_test;Password=bbhrm_test";
        using (var conn = new OracleConnection(connStr))
        {
            conn.Open();
            Console.WriteLine("Connected to Oracle");

            string[] cmds = new[]
            {
                "BEGIN EXECUTE IMMEDIATE 'DROP SEQUENCE CICDTest_PRODUCTS_SEQ'; EXCEPTION WHEN OTHERS THEN NULL; END;",
                "BEGIN EXECUTE IMMEDIATE 'DROP TRIGGER CICDTest_PRODUCTS_TRG'; EXCEPTION WHEN OTHERS THEN NULL; END;",
                "BEGIN EXECUTE IMMEDIATE 'DROP TABLE CICDTest_PRODUCTS'; EXCEPTION WHEN OTHERS THEN NULL; END;",
                "CREATE TABLE CICDTest_PRODUCTS (ID NUMBER(10) NOT NULL, NAME VARCHAR2(200) NOT NULL, DESCRIPTION VARCHAR2(1000), PRICE NUMBER(18,2) NOT NULL, STOCK_QUANTITY NUMBER(10) NOT NULL, IS_ACTIVE NUMBER(1) DEFAULT 1 NOT NULL, CREATED_AT TIMESTAMP DEFAULT SYSTIMESTAMP NOT NULL, UPDATED_AT TIMESTAMP, CONSTRAINT PK_CICDTest_PRODUCTS PRIMARY KEY (ID))",
                "CREATE SEQUENCE CICDTest_PRODUCTS_SEQ START WITH 1 INCREMENT BY 1",
                "CREATE OR REPLACE TRIGGER CICDTest_PRODUCTS_TRG BEFORE INSERT ON CICDTest_PRODUCTS FOR EACH ROW BEGIN SELECT CICDTest_PRODUCTS_SEQ.NEXTVAL INTO :NEW.ID FROM DUAL; END;",
                "INSERT INTO CICDTest_PRODUCTS (NAME, DESCRIPTION, PRICE, STOCK_QUANTITY) VALUES ('Laptop', 'High-performance laptop', 999.99, 50)",
                "INSERT INTO CICDTest_PRODUCTS (NAME, DESCRIPTION, PRICE, STOCK_QUANTITY) VALUES ('Mouse', 'Wireless mouse', 29.99, 200)",
                "INSERT INTO CICDTest_PRODUCTS (NAME, DESCRIPTION, PRICE, STOCK_QUANTITY) VALUES ('Keyboard', 'Mechanical keyboard', 79.99, 100)",
                "COMMIT"
            };

            foreach (var sql in cmds)
            {
                using (var cmd = new OracleCommand(sql, conn))
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("OK: " + sql.Substring(0, Math.Min(60, sql.Length)));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERR: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                    }
                }
            }

            using (var cmd = new OracleCommand("SELECT COUNT(*) FROM CICDTest_PRODUCTS", conn))
            {
                var count = cmd.ExecuteScalar();
                Console.WriteLine("Done! Rows in CICDTest_PRODUCTS: " + count);
            }
        }
    }
}
