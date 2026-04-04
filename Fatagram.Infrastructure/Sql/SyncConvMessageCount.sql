UPDATE conversations c
SET last_message_number = sub.last_message_number
FROM (
    SELECT conversation_id, COUNT(*) AS last_message_number
    FROM messages
    GROUP BY conversation_id
) sub
WHERE c.id = sub.conversation_id;
