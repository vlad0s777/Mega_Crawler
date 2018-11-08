ALTER TABLE public."ArticleTag" RENAME TO articles_tags;
ALTER TABLE articles_tags RENAME COLUMN "ArticleId" TO article_id;
ALTER TABLE articles_tags RENAME COLUMN "TagId" TO tag_id;

ALTER TABLE public."RemovedTags" RENAME TO removed_tags;
ALTER TABLE removed_tags RENAME COLUMN "RemovedTagId" TO removed_tag_id;
ALTER TABLE removed_tags RENAME COLUMN "TagId" TO tag_id;
ALTER TABLE removed_tags RENAME COLUMN "DeletionDate" TO deletion_date;

ALTER TABLE public."Articles" RENAME TO articles;
ALTER TABLE articles RENAME COLUMN "ArticleId" TO article_id;
ALTER TABLE articles RENAME COLUMN "DateCreate" TO date_create;
ALTER TABLE articles RENAME COLUMN "Text" TO text;
ALTER TABLE articles RENAME COLUMN "Head" TO head;
ALTER TABLE articles RENAME COLUMN "OuterArticleId" TO outer_article_id;

ALTER TABLE public."Tags" RENAME TO tags;
ALTER TABLE tags RENAME COLUMN "TagId" TO tag_id;
ALTER TABLE tags RENAME COLUMN "TagKey" TO tag_key;
ALTER TABLE tags RENAME COLUMN "Name" TO name;

ALTER TABLE articles_tags DROP CONSTRAINT "PK_ArticleTag";
DROP INDEX "IX_ArticleTag_TagId";
ALTER TABLE articles_tags ADD COLUMN id SERIAL PRIMARY KEY;