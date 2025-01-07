import { Component, inject, OnInit, signal } from '@angular/core';
import { AdminService } from '../../_services/admin.service';
import { Photo } from '../../_models/photos';

@Component({
  selector: 'app-photo-management',
  standalone: true,
  imports: [],
  templateUrl: './photo-management.component.html',
  styleUrl: './photo-management.component.css',
})
export class PhotoManagementComponent implements OnInit{
  private adminServie = inject(AdminService);
  photos: Photo[] = [];


  ngOnInit(): void {
    this.loadUnApprovedPhotos();
  }
  
  loadUnApprovedPhotos() {
    this.adminServie.getPhotosForApproval().subscribe({
      next: (photos) => (this.photos = photos),
    });
  }

  approvePhoto(photoId: number) {
    this.adminServie.approvePhoto(photoId).subscribe({
      next: () =>
        this.photos.splice(
          this.photos.findIndex((p) => p.id === photoId),
          1
        ),
    });
  }

  rejectPhoto(photoId: number) {
    this.adminServie.rejectPhoto(photoId).subscribe({
      next: () =>
        this.photos.splice(
          this.photos.findIndex((p) => p.id === photoId),
          1
        ),
    });
  }
}
