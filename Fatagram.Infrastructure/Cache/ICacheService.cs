using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Fatagram.Infrastructure.Cache
{
    /// <summary>
    /// Interface điều phối toàn bộ "bộ nhớ đệm" Redis cho Fatagram.
    /// Giúp hệ thống chạy mượt, giảm tải tối đa cho SQL Server.
    /// </summary>
    public interface ICacheService
    {
        // ==========================================
        // 1. BASIC STRING OPERATIONS (Dạng Key-Value đơn giản)
        // Phù hợp lưu: Thông tin Profile User, Token, hoặc các Flag đơn giản.
        // ==========================================

        /// <summary> Lấy dữ liệu từ Cache theo Key. </summary>
        Task<T?> GetAsync<T>(string key);

        /// <summary> Lưu dữ liệu vào Cache. Có thể set thời gian hết hạn (Expiration). </summary>
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

        /// <summary> Xóa vĩnh viễn một Key khỏi Cache. </summary>
        Task RemoveAsync(string key);

        /// <summary> Kiểm tra xem một Key có đang tồn tại trong Redis không. </summary>
        Task<bool> ExistsAsync(string key);

        // ==========================================
        // 2. HASH OPERATIONS (Dạng bảng nhỏ trong 1 Key)
        // Phù hợp lưu: Metadata của hội thoại, cài đặt riêng của User (Settings).
        // ==========================================

        /// <summary> Lưu một trường (field) vào trong một Key dạng bảng (Hash). </summary>
        Task HashSetAsync(string key, string field, string value);
        Task HashSetAsync(string key, HashEntry[] hashEntries);

        /// <summary> Lấy giá trị của một trường cụ thể trong bảng. </summary>
        Task<string?> HashGetAsync(string key, string field);

        /// <summary> Lấy toàn bộ các cặp Field-Value trong bảng đó về. </summary>
        Task<Dictionary<string, string>> HashGetAllAsync(string key);

        Task HashObjectSetAsync<T>(string key, string field, T value);
        Task<T?> HashObjectGetAsync<T>(string key, string field);

        // ==========================================
        // 3. SET OPERATIONS (Tập hợp KHÔNG TRÙNG LẶP)
        // "Vũ khí" chính để làm: Danh sách User Online, Danh sách đoạn chat chưa đọc.
        // ==========================================

        /// <summary> Thêm ID vào tập hợp. Nếu ID đã có rồi, Redis tự động lờ đi (không trùng). </summary>
        Task SetAddAsync(string key, string value);

        /// <summary> Xóa một ID cụ thể khỏi tập hợp (Ví dụ: Đánh dấu đã đọc phòng chat này). </summary>
        Task<long> SetRemoveAsync(string key, string value);

        /// <summary> Đếm xem có bao nhiêu phần tử trong tập hợp (Ví dụ: Có bao nhiêu đoạn chat chưa đọc). </summary>
        Task<long> SetLengthAsync(string key);

        /// <summary> Lấy ra toàn bộ danh sách ID có trong tập hợp đó. </summary>
        Task<IEnumerable<string>> SetMembersAsync(string key);

        // ==========================================
        // 4. UTILITIES (Tiện ích bổ sung)
        // ==========================================

        /// <summary> Tìm kiếm các Key theo mẫu (Pattern). Ví dụ: "conv:*:messages". </summary>
        Task<IEnumerable<string>> GetKeysAsync(string pattern);

        /// <summary> Tăng giá trị số của một Key (Atomic). Dùng để lấy SequenceNumber cho tin nhắn cực chuẩn. </summary>
        Task<long> IncrementAsync(string key, long value = 1);

        /// <summary> Tăng số lượng của một trường trong bảng Hash (Dùng đếm Unread Count cho từng User). </summary>
        Task<long> HashIncrementAsync(string key, string field, long value = 1);

        /// <summary> Khóa tạm thời một tài nguyên (Distributed Lock) để tránh tranh chấp dữ liệu giữa các Worker. </summary>
        Task<bool> TryAcquireLockAsync(string key, TimeSpan expiration);

        /// <summary>
        /// Thiết lập thời gian hết hạn cho một Key bất kỳ (String, Set, Hash, List).
        /// Giúp giải phóng bộ nhớ RAM cho Redis sau một khoảng thời gian.
        /// </summary>
        Task<bool> KeyExpireAsync(string key, TimeSpan expiration);

        // ==========================================
        // 5. LIST OPERATIONS (Hàng đợi - Queue)
        // Phù hợp làm: Batching Messages (Hàng chờ lưu vào SQL Server).
        // ==========================================

        /// <summary> Đẩy tin nhắn mới vào cuối hàng đợi (Tin mới nhất). </summary>
        Task<long> ListRightPushAsync<T>(string key, T value);

        /// <summary> Đẩy tin nhắn vào đầu hàng đợi. </summary>
        Task<long> ListLeftPushAsync<T>(string key, T value);

        /// <summary> Lấy một đoạn tin nhắn trong hàng đợi (Ví dụ: Lấy 50 tin để lưu xuống DB). </summary>
        Task<List<T>> ListRangeAsync<T>(string key, long start, long stop);

        /// <summary> Cắt tỉa hàng đợi. Dùng để xóa tin nhắn đã được lưu vào SQL thành công. </summary>
        Task ListTrimAsync(string key, long start, long stop);

        /// <summary> Kiểm tra xem hiện có bao nhiêu tin nhắn đang nằm "treo" trong hàng chờ. </summary>
        Task<long> ListLengthAsync(string key);

        // ==========================================
        // 6. SORTED SET OPERATIONS (Tập hợp CÓ THỨ TỰ)
        // Phù hợp làm: Bảng xếp hạng, Trending Posts, Lấy tin nhắn theo thời gian.
        // ==========================================

        /// <summary> Thêm phần tử vào tập hợp kèm điểm số (Score). </summary>
        Task<bool> SortedSetAddAsync<T>(string key, T value, double score);
        Task<bool> SortedSetAddAsync(string key, SortedSetEntry[] entries);

        /// <summary> Lấy danh sách phần tử trong khoảng điểm (Ví dụ: Lấy top 10 bài viết nhiều tương tác nhất). </summary>
        Task<List<T>> SortedSetRangeByScoreAsync<T>(
            string key,
            double start = double.NegativeInfinity,
            double stop = double.PositiveInfinity,
            Exclude exclude = Exclude.None,
            Order order = Order.Ascending,
            long offset = 0,
            long take = -1
        );

        /// <summary> Xóa một phần tử khỏi Sorted Set. </summary>
        Task<bool> SortedSetRemoveAsync<T>(string key, T value);

        /// <summary> Lấy thứ hạng (Rank) của một phần tử. </summary>
        Task<long?> SortedSetRankAsync<T>(string key, T value, bool desc = false);

        /// <summary> Lấy phần tử từ Sorted Set theo cursor (score threshold) để hỗ trợ pagination. </summary>
        Task<List<(T Value, double Score)>> SortedSetRangeByScoreWithCursorAsync<T>(
            string key,
            double? cursor = null,
            int limit = 20,
            bool desc = true
        );

        // ==========================================
        // 7. PUB/SUB OPERATIONS (Thông báo thời gian thực)
        // Phù hợp: Gửi thông báo giữa các Server (Scaling), Real-time Events.
        // ==========================================

        /// <summary> Phát tin nhắn đến một Channel cụ thể. </summary>
        Task<long> PublishAsync<T>(string channel, T message);

        /// <summary> Đăng ký lắng nghe một Channel (Thường dùng trong Background Service). </summary>
        Task SubscribeAsync<T>(string channel, Action<T> handler);

        // ==========================================
        // 8. ADVANCED / TRANSACTIONS
        // ==========================================

        /// <summary> Thực thi nhiều lệnh cùng lúc để giảm Round-trip time (RTT). </summary>
        Task ExecuteBatchAsync(Action<ICacheService> batchAction);
    }
}
