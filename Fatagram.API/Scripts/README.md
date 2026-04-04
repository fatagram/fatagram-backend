# Script mode (không cần chạy app)

Bạn có thể chạy script bằng DI của project API mà **không khởi động web server**:

```bash
dotnet run --project Fatagram.API -- script ping
dotnet run --project Fatagram.API -- script ping hello world
```

## Thêm script mới

Tạo class trong thư mục `Fatagram.API/Scripts` implement `ICommand`:

- `Name`: tên command bạn sẽ gọi
- `Execute(string[] args)`: logic script

Ví dụ:

```csharp
public class MyTaskCommand : ICommand
{
    public string Name => "my-task";

    public async Task Execute(string[] args)
    {
        // your logic
    }
}
```

Sau đó chạy:

```bash
dotnet run --project Fatagram.API -- script my-task
```
