import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { map } from 'rxjs';
import { MangaService } from '../../services/manga.service';

@Component({
  selector: 'app-manga-detail',
  templateUrl: './manga-detail.component.html',
  styleUrls: ['./manga-detail.component.css']
})
export class MangaDetailComponent implements OnInit {
  manga: string | undefined;
  allChapters: any;

  constructor(private activatedRoute: ActivatedRoute,
    private mangaservice: MangaService) { }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params => {
      this.manga = params['mangaName'];
      this.mangaservice.getMangaDetails(this.manga!).subscribe(detail => this.allChapters = detail);
    });

  }
}



