# Fatagram Notification API Documentation

## Tổng quan

Hệ thống Notification của Fatagram hỗ trợ 2 phương thức:

- **REST API**: Lấy danh sách, đánh dấu đã đọc, xóa notifications
- **SignalR (WebSocket)**: Nhận notification real-time

---

## 1. REST API Endpoints

### Base URL

```
/api/v1/notification
```

### Authentication

Tất cả endpoints yêu cầu **Bearer Token** trong header:

```
Authorization: Bearer {access_token}
```

---

### 1.1. Get Notifications (Pagination)

**Endpoint:** `GET /api/v1/notification`

**Description:** Lấy danh sách notifications với cursor-based pagination

**Query Parameters:**
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| cursor | DateTime (ISO 8601) | No | null | Cursor để phân trang (dùng `nextCursor` từ response trước) |
| limit | int | No | 10 | Số lượng notifications mỗi trang |

**Request Example:**

```http
GET /api/v1/notification?limit=20&cursor=2026-02-24T10:30:00Z
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Response Success (200):**

```json
{
  "statusCode": 200,
  "data": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "userId": "123e4567-e89b-12d3-a456-426614174000",
      "type": "NewFriendRequest",
      "actorId": "987fcdeb-51a2-43d7-b456-426614174999",
      "actorType": "User",
      "actorName": "Nguyễn Văn A",
      "actorImageUrl": "https://example.com/avatars/user-123.jpg",
      "link": "/friends/requests",
      "data": {
        "requestId": "abc123",
        "message": "Want to connect with you"
      },
      "isRead": false
    },
    {
      "id": "660e8400-e29b-41d4-a716-446655440001",
      "userId": "123e4567-e89b-12d3-a456-426614174000",
      "type": "FriendRequestAccepted",
      "actorId": "111fcdeb-51a2-43d7-b456-426614174888",
      "actorType": "User",
      "actorName": "Trần Thị B",
      "actorImageUrl": "https://example.com/avatars/user-456.jpg",
      "link": "/profile/111fcdeb-51a2-43d7-b456-426614174888",
      "data": {},
      "isRead": true
    }
  ],
  "nextCursor": "2026-02-23T15:20:00Z",
  "hasNext": true,
  "extraInfo": {},
  "message": null
}
```

**Cách sử dụng Pagination:**

```javascript
// First request
const firstPage = await fetch("/api/v1/notification?limit=20");
const firstData = await firstPage.json();

// Next pages - dùng nextCursor từ response trước
if (firstData.hasNext) {
  const secondPage = await fetch(
    `/api/v1/notification?limit=20&cursor=${firstData.nextCursor}`,
  );
  const secondData = await secondPage.json();
}
```

---

### 1.2. Get Unread Notifications

**Endpoint:** `GET /api/v1/notification/unread`

**Description:** Lấy danh sách notifications chưa đọc

**Query Parameters:** Giống như endpoint Get Notifications

**Request Example:**

```http
GET /api/v1/notification/unread?limit=10
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Response:** Giống format Get Notifications, nhưng chỉ trả về notifications có `isRead: false`

---

### 1.3. Mark Notification as Read

**Endpoint:** `POST /api/v1/notification/read/{notificationId}`

**Description:** Đánh dấu 1 notification cụ thể là đã đọc

**Path Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| notificationId | Guid | Yes | ID của notification |

**Request Example:**

```http
POST /api/v1/notification/read/550e8400-e29b-41d4-a716-446655440000
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Response Success (200):**

```json
{
  "statusCode": 200,
  "message": "OK"
}
```

---

### 1.4. Mark All Notifications as Read

**Endpoint:** `POST /api/v1/notification/read/all`

**Description:** Đánh dấu TẤT CẢ notifications là đã đọc

**Request Example:**

```http
POST /api/v1/notification/read/all
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Response Success (200):**

```json
{
  "statusCode": 200,
  "message": "OK"
}
```

---

### 1.5. Delete Notification

**Endpoint:** `DELETE /api/v1/notification/{notificationId}`

**Description:** Xóa 1 notification cụ thể

**Path Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| notificationId | Guid | Yes | ID của notification |

**Request Example:**

```http
DELETE /api/v1/notification/550e8400-e29b-41d4-a716-446655440000
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Response Success (200):**

```json
{
  "statusCode": 200,
  "message": "OK"
}
```

---

### 1.6. Delete All Notifications

**Endpoint:** `DELETE /api/v1/notification/all`

**Description:** Xóa TẤT CẢ notifications của user

**Request Example:**

```http
DELETE /api/v1/notification/all
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Response Success (200):**

```json
{
  "statusCode": 200,
  "message": "OK"
}
```

---

## 2. SignalR Real-time Notifications

### 2.1. Connection Setup

**Hub URL:**

```
/hubs/notification
```

**Authentication:** Gửi access token khi kết nối

#### JavaScript/TypeScript Example (using @microsoft/signalr)

```typescript
import * as signalR from "@microsoft/signalr";

class NotificationService {
  private connection: signalR.HubConnection;

  constructor(accessToken: string) {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl("https://api.fatagram.com/hubs/notification", {
        accessTokenFactory: () => accessToken,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .withAutomaticReconnect() // Auto reconnect khi mất kết nối
      .configureLogging(signalR.LogLevel.Information)
      .build();
  }

  async start() {
    try {
      await this.connection.start();
      console.log("✅ SignalR Connected");

      // Lắng nghe notification từ server
      this.connection.on("ReceiveNotification", (notification) => {
        console.log("📩 New notification:", notification);
        this.handleNewNotification(notification);
      });
    } catch (error) {
      console.error("❌ SignalR Connection Error:", error);
      // Retry after 5 seconds
      setTimeout(() => this.start(), 5000);
    }
  }

  handleNewNotification(notification: NotificationDto) {
    // Display notification (toast, badge, etc.)
    // Update notification list
    // Play sound
    // Show badge count
  }

  async stop() {
    await this.connection.stop();
  }
}

// Usage
const notificationService = new NotificationService(accessToken);
await notificationService.start();
```

---

### 2.2. Events

#### Server -> Client Events

| Event Name            | Description                            | Payload                  |
| --------------------- | -------------------------------------- | ------------------------ |
| `ReceiveNotification` | Server gửi notification mới cho client | `NotificationDto` object |

**Event Handler Example:**

```typescript
connection.on("ReceiveNotification", (notification: NotificationDto) => {
  // notification object structure giống với REST API response
  console.log("New notification:", notification);

  // Update UI
  displayToast(notification);
  updateNotificationBadge();
  playNotificationSound();
});
```

---

### 2.3. Connection Lifecycle

```typescript
// Connection Started
connection.onclose((error) => {
  console.error("Connection closed:", error);
  // Attempt to reconnect
});

// Reconnecting
connection.onreconnecting((error) => {
  console.warn("Reconnecting...", error);
  // Show UI indicator
});

// Reconnected
connection.onreconnected((connectionId) => {
  console.log("Reconnected:", connectionId);
  // Hide UI indicator
  // Fetch missed notifications during disconnect
  fetchMissedNotifications();
});
```

---

## 3. Data Structures

### 3.1. NotificationDto

```typescript
interface NotificationDto {
  id: string; // UUID của notification
  userId: string; // UUID của user nhận notification
  type: NotificationType; // Loại notification (enum)
  actorId: string | null; // UUID của người thực hiện hành động
  actorType: string; // Loại actor: "User", "System", "Group", "Page"
  actorName: string | null; // Tên của actor (đã được attach từ DB)
  actorImageUrl: string | null; // Avatar URL của actor (đã được attach từ DB)
  link: string | null; // Deep link để navigate khi click
  data: Record<string, string>; // Additional data (key-value pairs)
  isRead: boolean; // Đã đọc hay chưa
}
```

---

### 3.2. NotificationType Enum

```typescript
enum NotificationType {
  NewFriendRequest = "NewFriendRequest", // Có người gửi lời mời kết bạn
  FriendRequestAccepted = "FriendRequestAccepted", // Lời mời kết bạn được chấp nhận
  FriendRequestCanceled = "FriendRequestCanceled", // Lời mời kết bạn bị hủy
  System = "System", // Thông báo từ hệ thống
  CancelNotification = "CancelNotification", // Hủy notification trước đó (để FE xóa/ẩn)
}
```

**Lưu ý về `CancelNotification`:**

Đây là notification đặc biệt được gửi qua SignalR (real-time) để báo cho FE xóa/ẩn một notification trước đó. Notification này **không được lưu vào database**.

**Khi nào nhận được `CancelNotification`?**

- Khi người gửi lời mời kết bạn **cancel/revoke** lời mời đó
- FE cần xử lý bằng cách xóa notification tương ứng khỏi danh sách

**Cấu trúc data:**

```typescript
{
  "type": "CancelNotification",
  "userId": "123e4567-e89b-12d3-a456-426614174000",
  "data": {
    "notificationId": "550e8400-e29b-41d4-a716-446655440000" // ID của notification cần xóa
  }
}
```

**Cách xử lý ở FE:**

```typescript
connection.on("ReceiveNotification", (notification: NotificationDto) => {
  if (notification.type === "CancelNotification") {
    // Lấy ID của notification cần xóa
    const notificationIdToRemove = notification.data.notificationId;

    // Xóa notification khỏi state/UI
    setNotifications((prev) =>
      prev.filter((n) => n.id !== notificationIdToRemove),
    );
  } else {
    // Xử lý notification bình thường
    setNotifications((prev) => [notification, ...prev]);
  }
});
```

---

### 3.3. ActorType

```typescript
enum ActorType {
  User = "User", // Người dùng thông thường
  System = "System", // Hệ thống
  Group = "Group", // Nhóm
  Page = "Page", // Trang
}
```

---

## 4. Implementation Guide

### 4.1. React/Next.js Example

```typescript
// hooks/useNotifications.ts
import { useState, useEffect, useCallback } from "react";
import * as signalR from "@microsoft/signalr";

interface UseNotificationsReturn {
  notifications: NotificationDto[];
  unreadCount: number;
  loading: boolean;
  hasNext: boolean;
  loadMore: () => Promise<void>;
  markAsRead: (id: string) => Promise<void>;
  markAllAsRead: () => Promise<void>;
  deleteNotification: (id: string) => Promise<void>;
}

export function useNotifications(accessToken: string): UseNotificationsReturn {
  const [notifications, setNotifications] = useState<NotificationDto[]>([]);
  const [nextCursor, setNextCursor] = useState<string | null>(null);
  const [hasNext, setHasNext] = useState(false);
  const [loading, setLoading] = useState(false);
  const [connection, setConnection] = useState<signalR.HubConnection | null>(
    null,
  );

  // Calculate unread count
  const unreadCount = notifications.filter((n) => !n.isRead).length;

  // Fetch notifications
  const fetchNotifications = async (cursor?: string) => {
    setLoading(true);
    try {
      const url = cursor
        ? `/api/v1/notification?limit=20&cursor=${cursor}`
        : "/api/v1/notification?limit=20";

      const response = await fetch(url, {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });

      const result = await response.json();

      if (cursor) {
        // Append to existing
        setNotifications((prev) => [...prev, ...result.data]);
      } else {
        // Replace (first load or refresh)
        setNotifications(result.data);
      }

      setNextCursor(result.nextCursor);
      setHasNext(result.hasNext);
    } catch (error) {
      console.error("Failed to fetch notifications:", error);
    } finally {
      setLoading(false);
    }
  };

  // Load more
  const loadMore = useCallback(async () => {
    if (hasNext && nextCursor) {
      await fetchNotifications(nextCursor);
    }
  }, [hasNext, nextCursor]);

  // Mark as read
  const markAsRead = async (id: string) => {
    try {
      await fetch(`/api/v1/notification/read/${id}`, {
        method: "POST",
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });

      // Update local state
      setNotifications((prev) =>
        prev.map((n) => (n.id === id ? { ...n, isRead: true } : n)),
      );
    } catch (error) {
      console.error("Failed to mark as read:", error);
    }
  };

  // Mark all as read
  const markAllAsRead = async () => {
    try {
      await fetch("/api/v1/notification/read/all", {
        method: "POST",
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });

      // Update local state
      setNotifications((prev) => prev.map((n) => ({ ...n, isRead: true })));
    } catch (error) {
      console.error("Failed to mark all as read:", error);
    }
  };

  // Delete notification
  const deleteNotification = async (id: string) => {
    try {
      await fetch(`/api/v1/notification/${id}`, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });

      // Update local state
      setNotifications((prev) => prev.filter((n) => n.id !== id));
    } catch (error) {
      console.error("Failed to delete notification:", error);
    }
  };

  // Setup SignalR connection
  useEffect(() => {
    const hubConnection = new signalR.HubConnectionBuilder()
      .withUrl("/hubs/notification", {
        accessTokenFactory: () => accessToken,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .withAutomaticReconnect()
      .build();

    hubConnection.on("ReceiveNotification", (notification: NotificationDto) => {
      console.log("📩 New notification:", notification);

      // Add to top of list
      setNotifications((prev) => [notification, ...prev]);

      // Show toast notification
      showToast(notification);
    });

    hubConnection
      .start()
      .then(() => console.log("✅ SignalR Connected"))
      .catch((err) => console.error("❌ SignalR Error:", err));

    setConnection(hubConnection);

    // Cleanup on unmount
    return () => {
      hubConnection.stop();
    };
  }, [accessToken]);

  // Initial fetch
  useEffect(() => {
    fetchNotifications();
  }, []);

  return {
    notifications,
    unreadCount,
    loading,
    hasNext,
    loadMore,
    markAsRead,
    markAllAsRead,
    deleteNotification,
  };
}
```

---

### 4.2. Component Example

```tsx
// components/NotificationList.tsx
import { useNotifications } from "@/hooks/useNotifications";
import { formatDistanceToNow } from "date-fns";
import { vi } from "date-fns/locale";

export function NotificationList() {
  const {
    notifications,
    unreadCount,
    loading,
    hasNext,
    loadMore,
    markAsRead,
    deleteNotification,
  } = useNotifications(accessToken);

  const handleNotificationClick = async (notification: NotificationDto) => {
    if (!notification.isRead) {
      await markAsRead(notification.id);
    }

    // Navigate to link
    if (notification.link) {
      router.push(notification.link);
    }
  };

  return (
    <div className="notification-list">
      <div className="header">
        <h3>Thông báo</h3>
        {unreadCount > 0 && <span className="badge">{unreadCount}</span>}
      </div>

      {loading && notifications.length === 0 ? (
        <div>Loading...</div>
      ) : (
        <div className="notifications">
          {notifications.map((notification) => (
            <div
              key={notification.id}
              className={`notification-item ${!notification.isRead ? "unread" : ""}`}
              onClick={() => handleNotificationClick(notification)}
            >
              {notification.actorImageUrl && (
                <img
                  src={notification.actorImageUrl}
                  alt={notification.actorName || ""}
                  className="avatar"
                />
              )}

              <div className="content">
                <p>
                  <strong>{notification.actorName}</strong>{" "}
                  {getNotificationMessage(notification.type)}
                </p>
                <span className="time">
                  {formatDistanceToNow(new Date(notification.createdAt), {
                    addSuffix: true,
                    locale: vi,
                  })}
                </span>
              </div>

              <button
                onClick={(e) => {
                  e.stopPropagation();
                  deleteNotification(notification.id);
                }}
                className="delete-btn"
              >
                ×
              </button>
            </div>
          ))}

          {hasNext && (
            <button onClick={loadMore} disabled={loading}>
              {loading ? "Đang tải..." : "Xem thêm"}
            </button>
          )}
        </div>
      )}
    </div>
  );
}

function getNotificationMessage(type: NotificationType): string {
  switch (type) {
    case "NewFriendRequest":
      return "đã gửi lời mời kết bạn";
    case "FriendRequestAccepted":
      return "đã chấp nhận lời mời kết bạn";
    case "FriendRequestCanceled":
      return "đã hủy lời mời kết bạn";
    default:
      return "";
  }
}
```

---

## 5. Best Practices

### 5.1. Performance Optimization

1. **Pagination**: Luôn sử dụng `limit` hợp lý (10-20 items/page)
2. **Caching**: Cache notifications đã fetch ở client-side
3. **Virtual Scrolling**: Sử dụng với danh sách notification dài
4. **Debounce**: Debounce mark as read khi scroll nhanh

### 5.2. Error Handling

```typescript
// Retry logic for SignalR
const startConnection = async (retries = 3) => {
  for (let i = 0; i < retries; i++) {
    try {
      await connection.start();
      return;
    } catch (error) {
      console.error(`Connection attempt ${i + 1} failed:`, error);
      if (i === retries - 1) throw error;
      await new Promise((resolve) => setTimeout(resolve, 2000 * (i + 1)));
    }
  }
};
```

### 5.3. User Experience

1. **Toast Notifications**: Hiển thị toast khi nhận notification mới
2. **Badge Counter**: Update badge count real-time
3. **Sound**: Play sound cho notification quan trọng (có thể tắt)
4. **Mark as read on view**: Tự động mark as read khi user nhìn thấy
5. **Deep linking**: Navigate đúng màn hình khi click notification

### 5.4. Security

1. Luôn validate access token trước khi connect SignalR
2. Refresh token khi expire và reconnect SignalR
3. Không lưu sensitive data trong notification.data
4. Validate notification từ server trước khi hiển thị

---

## 6. Error Codes

| Status Code | Description                               | Solution                                  |
| ----------- | ----------------------------------------- | ----------------------------------------- |
| 401         | Unauthorized - Token invalid hoặc expired | Refresh token và retry                    |
| 403         | Forbidden - User không có quyền           | Check user permissions                    |
| 404         | Notification not found                    | Notification đã bị xóa hoặc không tồn tại |
| 500         | Internal Server Error                     | Retry hoặc contact support                |

---

## 7. Testing

### 7.1. Test REST API với cURL

```bash
# Get notifications
curl -X GET "https://api.fatagram.com/api/v1/notification?limit=10" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"

# Mark as read
curl -X POST "https://api.fatagram.com/api/v1/notification/read/550e8400-e29b-41d4-a716-446655440000" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

### 7.2. Test SignalR Connection

```typescript
// Test if SignalR is connected
if (connection.state === signalR.HubConnectionState.Connected) {
  console.log("✅ Connected");
} else {
  console.log("❌ Not connected:", connection.state);
}
```

---

## 8. Migration Notes

Nếu đang migrate từ polling sang SignalR:

1. Keep REST API cho historical data
2. Use SignalR chỉ cho real-time updates
3. Fetch missed notifications khi reconnect
4. Implement offline queue nếu cần

---

## Support

Nếu có vấn đề, liên hệ Backend team hoặc tạo issue trên GitHub.

**Last Updated:** February 24, 2026
