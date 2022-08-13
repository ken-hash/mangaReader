import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-manga',
  templateUrl: './manga.component.html',
  styleUrls: ['./manga.component.css']
})
export class MangaComponent implements OnInit {
  allManga: any;

  constructor(private activatedRoute: ActivatedRoute,
    private http: HttpClient) { }

  ngOnInit(): void {
    let params = new HttpParams();
    params = params.append("max", 20);
    this.http.get('https://localhost:7235/api/Mangas/mangaList/', { params: params, }).subscribe(response => {
      this.allManga = response;
    }, error => {
      console.log(error);
    })
  }

}
