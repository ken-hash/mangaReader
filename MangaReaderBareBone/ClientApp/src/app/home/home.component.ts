import { Component, OnInit } from '@angular/core';
import { MangaLatest } from '../commons/models/mangaLatest.model';
import { MangaService } from '../services/manga.service';
import { UpdateService } from '../services/update.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  title = 'Manga Reader';
  latestManga: MangaLatest[] | undefined;
  readManga: MangaLatest[] | undefined;
  updates: string | undefined;

  constructor(private mangaService: MangaService, private updateService: UpdateService) { }

  ngOnInit() {
    this.fetchLatestChapters();
    this.fetchLastReadMangas();
    this.fetchUpdate();
  }

  private fetchLatestChapters() {
    this.mangaService.getLatestChapters(5).subscribe(
      (latestChapters: MangaLatest[]) => this.latestManga = latestChapters,
      (error : any) => console.error('Error fetching latest chapters:', error)
    );
  }

  private fetchLastReadMangas() {
    this.mangaService.getLastReadMangas(5).subscribe(
      (lastReadChapters: MangaLatest[]) => this.readManga = lastReadChapters,
      (error: any) => console.error('Error fetching last read mangas:', error)
    );
  }

  private fetchUpdate() {
    this.updateService.getUpdate('asura').subscribe(
      (latestUpdate: Date) => {
        const formattedDate = latestUpdate.toLocaleDateString('en-US');
        const formattedTime = latestUpdate.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
        this.updates = `${formattedDate} ${formattedTime}`;
      },
      (error) => console.error('Error fetching update:', error)
    );
  }
}
