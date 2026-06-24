-- Создание БД
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'OrderAccountingDB')
BEGIN
    CREATE DATABASE OrderAccountingDB;
END;
GO

USE OrderAccountingDB;
GO

-- Заказы
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Orders (
        OrderNumber INT IDENTITY(1,1) PRIMARY KEY,
        OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
        Amount DECIMAL(18,2) NOT NULL CHECK (Amount >= 0),
        PaidAmount DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (PaidAmount >= 0),
        RowVersion ROWVERSION NOT NULL
    );
END;
GO

-- Приход денег
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CashInflows' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.CashInflows (
        InflowNumber INT IDENTITY(1,1) PRIMARY KEY,
        InflowDate DATETIME NOT NULL DEFAULT GETDATE(),
        Amount DECIMAL(18,2) NOT NULL CHECK (Amount > 0),
        Remaining DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (Remaining >= 0),
        RowVersion ROWVERSION NOT NULL
    );
END;
GO

-- Платежи
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Payments' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Payments (
        PaymentID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        OrderNumber INT NOT NULL,
        InflowNumber INT NOT NULL,
        PaymentAmount DECIMAL(18,2) NOT NULL CHECK (PaymentAmount > 0),
        PaymentDate DATETIME NOT NULL DEFAULT GETDATE(),
        ExpectedOrderVersion VARBINARY(8) NULL,
        ExpectedInflowVersion VARBINARY(8) NULL,
        CONSTRAINT FK_Payments_Orders FOREIGN KEY (OrderNumber) REFERENCES dbo.Orders(OrderNumber),
        CONSTRAINT FK_Payments_CashInflows FOREIGN KEY (InflowNumber) REFERENCES dbo.CashInflows(InflowNumber)
    );
END;
GO

-- Триггер на новые платежи

CREATE OR ALTER TRIGGER TR_Payments_ApplyPayment
ON Payments
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Проверка RowVersion для оптимистичного лока + бизнес-логика
        IF EXISTS (
            SELECT 1 
            FROM inserted i
            LEFT JOIN Orders o ON o.OrderNumber = i.OrderNumber
            WHERE o.OrderNumber IS NULL 
               OR (i.ExpectedOrderVersion IS NOT NULL AND o.RowVersion <> i.ExpectedOrderVersion)
               OR o.PaidAmount + i.PaymentAmount > o.Amount
        )
        BEGIN
            RAISERROR('Данные заказа были изменены другим пользователем или сумма оплаты превышена.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END;
        
        IF EXISTS (
            SELECT 1 
            FROM inserted i
            LEFT JOIN CashInflows ci ON ci.InflowNumber = i.InflowNumber
            WHERE ci.InflowNumber IS NULL 
               OR (i.ExpectedInflowVersion IS NOT NULL AND ci.RowVersion <> i.ExpectedInflowVersion)
               OR ci.Remaining < i.PaymentAmount
        )
        BEGIN
            RAISERROR('Данные прихода были изменены другим пользователем или недостаточно средств.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END;
        
        -- Обновление данных
        UPDATE o
        SET PaidAmount = PaidAmount + i.PaymentAmount
        FROM Orders o
        INNER JOIN inserted i ON o.OrderNumber = i.OrderNumber;
        
        UPDATE ci
        SET Remaining = Remaining - i.PaymentAmount
        FROM CashInflows ci
        INNER JOIN inserted i ON ci.InflowNumber = i.InflowNumber;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
