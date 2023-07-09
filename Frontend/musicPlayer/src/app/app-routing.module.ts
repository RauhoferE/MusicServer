import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './views/user/login/login.component';
import { RouterModule, Routes } from '@angular/router';
import { RegisterComponent } from './views/user/register/register.component';
import { PlaylistsComponent } from './views/playlist/playlists/playlists.component';
import { isAuthenticatedGuard } from './route-guards/is-authenticated.guard';
import { NotFoundComponent } from './views/common/not-found/not-found.component';
import { isUnknownGuard } from './route-guards/is-unknown.guard';

const routes: Routes = [
  { 
    path: 'login',
    component: LoginComponent,
    canActivate: [isUnknownGuard]
  },
  { 
    path: 'register',
    component: RegisterComponent,
    canActivate: [isUnknownGuard]
  },
  { 
    path: 'playlists',
    component: PlaylistsComponent,
    canActivate: [isAuthenticatedGuard]
  },
  { 
    path: '',
    redirectTo: '/playlists',
    pathMatch: 'full'
  },
  { 
    path: '**',
    component: NotFoundComponent
  },
];

@NgModule({
  declarations: [],
  imports: [
    RouterModule.forRoot(routes)
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
