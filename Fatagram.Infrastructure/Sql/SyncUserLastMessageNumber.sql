update conversation_participants cp
set last_seen_number = sub.last_seen_number
from (
    select cp.conversation_id, cp.user_id, count(*) as last_seen_number
    from conversation_participants cp
    join messages m on cp.last_seen_message_id = m.id
    join messages m2 on m2.conversation_id = cp.conversation_id
    where m.created_at <= m.created_at
    group by cp.conversation_id, cp.user_id
) sub
where cp.conversation_id = sub.conversation_id
and cp.user_id = sub.user_id;