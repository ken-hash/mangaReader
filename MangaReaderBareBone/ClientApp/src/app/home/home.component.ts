import { Component, OnInit } from '@angular/core';
import { MangaLatest } from '../commons/models/mangaLatest.model';
import { MangaService } from '../services/manga.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})

export class HomeComponent implements OnInit {
  title = 'Manga Reader';
  latestManga: MangaLatest[] | undefined;
  readManga: MangaLatest[] | undefined;

  constructor(private mangaService: MangaService) { }

  ngOnInit() {
    this.fetchLatestChapters();
    this.fetchLastReadMangas();
  }

  private fetchLatestChapters() {
    this.mangaService.getLatestChapters(5).subscribe(
      (latestChapters: MangaLatest[]) => (this.latestManga = latestChapters)
    );
  }

  private fetchLastReadMangas() {
    this.mangaService.getLastReadMangas(5).subscribe(
      (lastReadChapters: MangaLatest[]) => (this.readManga = lastReadChapters)
    );
  }
}
