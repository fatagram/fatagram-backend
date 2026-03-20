CREATE OR REPLACE FUNCTION fn_send_unified_message(
    p_sender_id UUID,
    p_recipient_id UUID,
    p_message_content TEXT
)
RETURNS TABLE (conversation_id UUID, is_new_conversation BOOLEAN) AS $$
DECLARE
    v_conversation_id UUID;
    v_is_new_conversation BOOLEAN := FALSE;
BEGIN
    SELECT conversation_id INTO v_conversation_id
    FROM conversation_participants
    GROUP BY conversation_id
    HAVING COUNT(*) = 2
    AND COUNT(*) FILTER (WHERE user_id IN (p_sender_id, p_recipient_id)) = 2
    LIMIT 1;

    if v_conversation_id is null then
        v_conversation_id := gen_random_uuid();
        insert into conversations (id, is_group) values (v_conversation_id, FALSE);
        insert into conversation_participants (conversation_id, user_id) values 
        (v_conversation_id, p_sender_id), (v_conversation_id, p_recipient_id);
        v_is_new_conversation := TRUE;
    end if;

    insert into messages (id, conversation_id, sender_id, content) values 
    (gen_random_uuid(), v_conversation_id, p_sender_id, p_message_content);

    return query select v_conversation_id, v_is_new_conversation;
END;
$$ LANGUAGE plpgsql;