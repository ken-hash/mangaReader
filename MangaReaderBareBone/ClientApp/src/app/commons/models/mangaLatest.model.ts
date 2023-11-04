import { MangaChapter } from "./mangaChapter.model";

export class MangaLatest {
  constructor(public mangaId: string, public name: string, public chapters: MangaChapter[]) { }
}
