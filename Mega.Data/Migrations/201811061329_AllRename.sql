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