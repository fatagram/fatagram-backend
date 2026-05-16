-- Migrate all 1-to-1 conversations to have a simple string unique_conversation_key (id1:id2)
-- Logic matches C#: id1:id2 (sorted alphabetically)

WITH participants_ordered AS (
    SELECT 
        conversation_id,
        user_id,
        row_number() OVER (PARTITION BY conversation_id ORDER BY user_id) as rn
    FROM conversation_participants
),
keys AS (
    SELECT 
        p1.conversation_id,
        CASE 
            WHEN p2.user_id IS NOT NULL THEN
                LEAST(p1.user_id, p2.user_id)::text || ':' || GREATEST(p1.user_id, p2.user_id)::text
            ELSE
                p1.user_id::text || ':' || p1.user_id::text
        END as generated_key
    FROM participants_ordered p1
    LEFT JOIN participants_ordered p2 ON p1.conversation_id = p2.conversation_id AND p2.rn = 2
    WHERE p1.rn = 1
)
UPDATE conversations c
SET unique_conversation_key = k.generated_key
FROM keys k
WHERE c.id = k.conversation_id AND c.is_group = false;
