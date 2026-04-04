with sequence_number as (
    select id, row_number() over (
        partition by conversation_id
        order by created_at asc
    ) as new_sequence_number
    from messages
)
update messages m
set sequence_number = s.new_sequence_number
from sequence_number s
where m.id = s.id;