import java.awt.image.BufferedImage;
import java.awt.image.DataBufferByte;
import java.io.File;
import java.io.IOException;
import javax.imageio.*;

class PrintImage{
    public static void main(String args[]){    
        if (args.length < 3) {
            System.out.println("Usage: java PrintImage <image file> <width> <height>");
            return;    
        }
		int[][] data = toGrayscaleArray(args[0]);
		if (data != null) {
            printASCIIImage(data, Integer.parseInt(args[1]), Integer.parseInt(args[2])); 
        }
    }
    private static void printASCIIImage(int[][] image, int width, int height){
        final String grayscale = "$@B%8&WM#*oahkbdpqwmZO0QLCJUYXzcvunxrjft/\\|()1{}[]?-_+~<>i!lI;:,\"^`\'.      ";
        if (width > image[0].length)
            width = image[0].length;
        if (height > image.length)
            height = image.length;
        int yRange = image.length / height;
        int xRange = image[0].length / width;
        int blockSize= xRange * yRange;
        for (int row = 0; row <= image.length - yRange; row+= yRange) { 
            for (int col = 0; col <= image[0].length - xRange; col+= xRange) {
                int sum = 0;
                for (int y = 0; y < yRange; y++){
                    for (int x = 0; x < xRange; x++){
                        sum += image[row+y][col+x];
                    }
                }
                sum = sum / blockSize;
                System.out.print(grayscale.charAt(grayscale.length() - 1 - grayscale.length() * sum / 255));
            }
            System.out.println();
        }
    }
    private static int[][] toGrayscaleArray(String fileName){
            BufferedImage img = null;
            try {
                img = ImageIO.read(new File(fileName));
            } catch (IOException e) {
                System.out.println(e);
                return null;
            }
            byte[] pixels = ((DataBufferByte)img.getRaster().getDataBuffer()).getData();
            int width = img.getWidth();
            int[][] ret = new int[img.getHeight()][width];
            boolean hasAlphaChannel = img.getAlphaRaster() != null;
            if (hasAlphaChannel){
                int pixelLength = 4;
                for (int pixel = 0, row = 0, col = 0; pixel < pixels.length; pixel += pixelLength){
                    float grayscale = 0;
                    grayscale += 0.0722 * (pixels[pixel + 1] & 0xff);    //blue
                    grayscale += 0.7152 * (pixels[pixel + 2] & 0xff);    //green
                    grayscale += 0.2126 * (pixels[pixel + 3] & 0xff);    //red
                    int alpha = (int) pixels[pixel] & 0xff;    //alpha
                    ret[row][col] = (int)(grayscale / 3 * alpha / 255); 
                    col++;
                    if (col == width) {
                        col = 0;
                        row++;
                    }
                }
            } else {
                int pixelLength = 3;
                for (int pixel = 0, row = 0, col = 0; pixel < pixels.length; pixel += pixelLength){
                    int grayscale = 0;
                    grayscale += 0.0722 * (pixels[pixel] & 0xff);    //blue
                    grayscale += 0.7152 * (pixels[pixel + 1] & 0xff);    //green
                    grayscale += 0.2126 * (pixels[pixel + 2] & 0xff);    //red
                    ret[row][col] = (int)(grayscale / 3);
                    col++;
                    if (col == width) {
                        col = 0;
                        row++;
                    }
                }
            }
            return ret;
    }
}