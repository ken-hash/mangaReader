import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { MangaComponent } from './manga/manga.component';
import { MangaDetailComponent } from './manga/manga-detail/manga-detail.component';
import { MangaReadComponent } from './manga/manga-read/manga-read.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    MangaComponent,
    MangaDetailComponent,
    MangaReadComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'Manga', component: MangaComponent },
      { path: 'Manga/:mangaName', component: MangaDetailComponent },
      { path: 'Manga/:mangaName/:chapterName', component: MangaReadComponent }
    ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }