UPDATE conversations c
SET search_text = (
    SELECT 
        LOWER(unaccent(
            STRING_AGG(COALESCE(u.full_name, '') || ' ' || COALESCE(cp.nickname, ''), ' ') || ' ' ||
            COALESCE(c.name, '')
        ))
    FROM conversation_participants cp
    JOIN users u ON cp.user_id = u.id
    WHERE cp.conversation_id = c.id
);
