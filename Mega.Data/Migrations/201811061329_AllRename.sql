ALTER TABLE public."ArticleTag" RENAME TO articles_tags;
ALTER TABLE public.articles_tags RENAME COLUMN "ArticleId" TO article_id;
ALTER TABLE public.articles_tags RENAME COLUMN "TagId" TO tag_id;

ALTER TABLE public."RemovedTags" RENAME TO removed_tags;
ALTER TABLE public.removed_tags RENAME COLUMN "RemovedTagId" TO removed_tag_id;
ALTER TABLE public.removed_tags RENAME COLUMN "TagId" TO tag_id;
ALTER TABLE public.removed_tags RENAME COLUMN "DeletionDate" TO deletion_date;

ALTER TABLE public."Articles" RENAME TO articles;
ALTER TABLE public.articles RENAME COLUMN "ArticleId" TO article_id;
ALTER TABLE public.articles RENAME COLUMN "DateCreate" TO date_create;
ALTER TABLE public.articles RENAME COLUMN "Text" TO text;
ALTER TABLE public.articles RENAME COLUMN "Head" TO head;
ALTER TABLE public.articles RENAME COLUMN "OuterArticleId" TO outer_article_id;

ALTER TABLE public."Tags" RENAME TO tags;
ALTER TABLE public.tags RENAME COLUMN "TagId" TO tag_id;
ALTER TABLE public.tags RENAME COLUMN "TagKey" TO tag_key;
ALTER TABLE public.tags RENAME COLUMN "Name" TO name;


--ALTER TABLE articles_tags DROP CONSTRAINT "FK_ArticleTag_Articles_ArticleId"; 
--ALTER TABLE articles_tags DROP CONSTRAINT "FK_ArticleTag_Tags_TagId";
--ALTER TABLE articles_tags DROP CONSTRAINT "PK_ArticleTag";
--DROP INDEX "IX_ArticleTag_TagId";
--ALTER TABLE articles_tags ADD COLUMN id SERIAL PRIMARY KEY;
--CREATE UNIQUE INDEX ix_articletag ON articles_tags (article_id, tag_id);

--ALTER TABLE ONLY articles_tags ADD CONSTRAINT fk_articlestags_articles_articleid FOREIGN KEY (article_id) REFERENCES articles(article_id) ON DELETE CASCADE;
--ALTER TABLE ONLY articles_tags ADD CONSTRAINT fk_articlestags_tags_tagid FOREIGN KEY (tag_id) REFERENCES tags(tag_id) ON DELETE CASCADE;