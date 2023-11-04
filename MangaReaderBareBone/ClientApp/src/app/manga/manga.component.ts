import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute, NavigationStart, Router } from '@angular/router';
import { filter } from 'rxjs';
import { MangaList } from '../commons/models/mangaList.model';
import { MangaService } from '../services/manga.service';

@Component({
  selector: 'app-manga',
  templateUrl: './manga.component.html',
  styleUrls: ['./manga.component.css']
})
export class MangaComponent implements OnInit {
  allManga: any;

  constructor(private activatedRoute: ActivatedRoute,
    private mangaService: MangaService,
    private router: Router,
    private titleService: Title
  ) { }

  ngOnInit(): void {
    this.titleService.setTitle("MangaReader: Manga List");
    this.mangaService.getMangaList(1000).subscribe((list : MangaList) => this.allManga = list);
  }

}
