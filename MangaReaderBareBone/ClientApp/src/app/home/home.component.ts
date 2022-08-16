import { Component, OnInit } from '@angular/core';
import { MangaService } from '../services/manga.service';


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})

export class HomeComponent implements OnInit {
  title = 'Manga Reader';
  latestManga: any;
  readManga: any;

  constructor(private mangaService: MangaService) {
  }
  ngOnInit() {
    this.mangaService.getLatestChapters().subscribe(latestChapters => this.latestManga = latestChapters);
    this.mangaService.getLastReadMangas().subscribe(lastReadChapters => this.readManga = lastReadChapters);
  }
}
