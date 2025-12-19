# Fatagram Bulk Testing Tools

Console tool để tạo hàng loạt tài khoản test và gửi friend requests cho mục đích testing.

## Chức năng

1. **Bulk Account Creator**: Tạo nhiều tài khoản fake cùng lúc với dữ liệu ngẫu nhiên (sử dụng Bogus library)
2. **Bulk Friend Request Sender**: Gửi friend requests từ tất cả accounts đã tạo đến một target user

## Cách sử dụng

### 1. Build project

```bash
cd Fatagram.Tools
dotnet restore
dotnet build
```

### 2. Chạy tool

**Syntax:**

```bash
dotnet run <targetUserId> <numberOfAccounts> [connectionString]
```

**Parameters:**

- `targetUserId`: GUID của user sẽ nhận friend requests
- `numberOfAccounts`: Số lượng accounts muốn tạo (integer dương)
- `connectionString`: (Optional) Connection string đến database. Nếu không cung cấp, sẽ đọc từ `appsettings.json`

**Ví dụ:**

```bash
# Tạo 50 accounts và gửi friend requests đến user có ID chỉ định
dotnet run 550e8400-e29b-41d4-a716-446655440000 50

# Với custom connection string
dotnet run 550e8400-e29b-41d4-a716-446655440000 100 "Host=localhost;Port=5432;Database=fatagram;Username=postgres;Password=mypass"
```

### 3. Lấy Target User ID

Để lấy user ID của tài khoản muốn test:

**Option 1: Từ database**

```sql
SELECT id, username, full_name FROM users WHERE username = 'your_username';
```

**Option 2: Từ API response khi login**

```json
{
  "data": {
    "user": {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      ...
    }
  }
}
```

## Output

Tool sẽ:

1. ✅ Tạo N accounts với:
   - Username: `test_[random]_[guid]`
   - Email: `test_[guid]@test.com`
   - Password: `Test@123` (tất cả accounts)
   - Random first name, last name, gender, bio, avatar
2. ✅ Gửi friend requests từ tất cả accounts mới tạo đến target user
3. 📊 Hiển thị summary

**Example output:**

```
=== Fatagram Bulk Account Creator & Friend Request Tool ===

Target User ID: 550e8400-e29b-41d4-a716-446655440000
Number of accounts to create: 50

Target user found: John Doe (@johndoe)

Press ENTER to continue or CTRL+C to cancel...

Creating 50 accounts...

Created 50/50 accounts...

✅ Created 50 accounts successfully!

Sending friend requests to target user...

Sent 50/50 friend requests...

✅ Sent 50 friend requests successfully!

--- Summary ---
Created accounts: 50
Friend requests sent: 50
Default password for all accounts: Test@123

✅ Done!
```

## Thông tin quan trọng

⚠️ **Chỉ sử dụng cho môi trường DEV/TEST**

- Tool này tạo fake data và có thể làm đầy database
- Không chạy trên production!

🔑 **Default Password**: `Test@123`

- Tất cả accounts được tạo đều có password này
- Có thể login bằng username và password này

🗑️ **Cleanup**
Để xóa test data sau khi test xong:

```sql
-- Xóa friend requests từ test accounts
DELETE FROM friend_requests WHERE sender_id IN (
  SELECT id FROM users WHERE username LIKE 'test_%'
);

-- Xóa accounts
DELETE FROM accounts WHERE user_id IN (
  SELECT id FROM users WHERE username LIKE 'test_%'
);

-- Xóa users
DELETE FROM users WHERE username LIKE 'test_%';
```

## Configuration

Edit `appsettings.json` để set default connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=fatagram;Username=postgres;Password=yourpass"
  }
}
```

## Dependencies

- **Bogus**: Fake data generator
- **BCrypt.Net-Next**: Password hashing
- **Npgsql.EntityFrameworkCore.PostgreSQL**: PostgreSQL provider
- **Microsoft.Extensions.Configuration**: Configuration management

## Troubleshooting

**Error: "Target user not found"**

- Kiểm tra lại GUID format
- Verify user tồn tại trong database

**Error: "Database connection failed"**

- Check connection string
- Ensure PostgreSQL đang chạy
- Verify credentials

**Error: "Duplicate key violation"**

- Username/email đã tồn tại (rất hiếm với random GUID)
- Chạy lại tool
