import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MangaService } from '../services/manga.service';

@Component({
  selector: 'app-manga',
  templateUrl: './manga.component.html',
  styleUrls: ['./manga.component.css']
})
export class MangaComponent implements OnInit {
  allManga: any;

  constructor(private activatedRoute: ActivatedRoute,
    private mangaService: MangaService  ) { }

  ngOnInit(): void {
    this.mangaService.getMangaList(20).subscribe(list => this.allManga = list);
  }

}
