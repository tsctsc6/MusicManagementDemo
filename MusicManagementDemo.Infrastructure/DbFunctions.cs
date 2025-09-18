using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Domain.Entity.Music;

namespace MusicManagementDemo.Infrastructure;

internal static class DbFunctions
{
    public const string DefineGetMusicInfoInMusicListReturnType = $"""
        CREATE TYPE {DbSchemas.Music}.{DbSchemas.GetMusicInfoInMusicListReturnType} AS (
        "Id" UUID,
        "Title" character varying(200),
        "Artist" character varying(100),
        "Album" character varying(100)
        );
        """;
    public const string DefineGetMusicInfoInMusicList = $"""
      -- 定义函数
      CREATE OR REPLACE FUNCTION {DbSchemas.Music}.{DbSchemas.GetMusicInfoInMusicList}(
        music_list_id UUID,
          start_id UUID DEFAULT NULL,
          num_items INTEGER DEFAULT 10,
        is_desc BOOL DEFAULT false
      )
      RETURNS SETOF {DbSchemas.Music}.{DbSchemas.GetMusicInfoInMusicListReturnType}
      LANGUAGE plpgsql
      AS $$
      DECLARE
          current_id UUID := start_id;
          rec RECORD;
        result {DbSchemas.Music}.{DbSchemas.GetMusicInfoInMusicListReturnType};
      BEGIN
        IF current_id IS NOT NULL THEN
            SELECT m."PrevId", m."NextId" INTO rec
            FROM {DbSchemas.Music}."{nameof(MusicInfoMusicListMap)}" AS m
            WHERE m."MusicListId" = music_list_id AND m."MusicInfoId" = current_id;
            -- 更新当前 ID 为 NextId
            IF is_desc THEN
                current_id := rec."PrevId";
            ELSE
                current_id := rec."NextId";
            END IF;
        END IF;
          FOR i IN 1..num_items LOOP
            -- 查询当前记录
            IF current_id IS NULL THEN
                IF is_desc THEN
                    SELECT mi."Id", mi."Title", mi."Artist", mi."Album", m."PrevId", m."NextId" INTO rec
                    FROM {DbSchemas.Music}."{nameof(MusicInfoMusicListMap)}" AS m
                    JOIN {DbSchemas.Music}."{nameof(
		                  MusicInfo
	                  )}" AS mi ON m."MusicInfoId" = mi."Id"
                    WHERE m."MusicListId" = music_list_id AND m."NextId" is NULL;
                ELSE
                    SELECT mi."Id", mi."Title", mi."Artist", mi."Album", m."PrevId", m."NextId" INTO rec
                    FROM {DbSchemas.Music}."{nameof(MusicInfoMusicListMap)}" AS m
                    JOIN {DbSchemas.Music}."{nameof(
		                  MusicInfo
	                  )}" AS mi ON m."MusicInfoId" = mi."Id"
                    WHERE m."MusicListId" = music_list_id AND m."PrevId" is NULL;
                END IF;
            ELSE
                SELECT mi."Id", mi."Title", mi."Artist", mi."Album", m."PrevId", m."NextId" INTO rec
                FROM {DbSchemas.Music}."{nameof(MusicInfoMusicListMap)}" AS m
                JOIN {DbSchemas.Music}."{nameof(
	                  MusicInfo
                  )}" AS mi ON m."MusicInfoId" = mi."Id"
                WHERE m."MusicListId" = music_list_id AND m."MusicInfoId" = current_id;
              END IF;
              -- 如果未找到，退出循环
            IF NOT FOUND THEN
                EXIT;
            END IF;

            result."Id" := rec."Id";
              result."Title" := rec."Title";
              result."Artist" := rec."Artist";
            result."Album" := rec."Album";
            
              -- 返回当前元组
              RETURN Next result;
              
              -- 更新当前 ID 为 NextId
              IF is_desc THEN
                current_id := rec."PrevId";
            ELSE
                current_id := rec."NextId";
            END IF;
              
              -- 如果 NextId 为 NULL，退出循环
              IF current_id IS NULL THEN
                  EXIT;
              END IF;
          END LOOP;
      END;
      $$;
      """;
}
